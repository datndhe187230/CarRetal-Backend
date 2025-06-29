using CarRental_BE.Data;
using CarRental_BE.Exceptions;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using CarRental_BE.Services;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InvalidOperationException = CarRental_BE.Exceptions.InvalidOperationException;

namespace CarRental_BE.Repositories.Impl
{
    public class UserRepositoryImpl : IUserRepository
    {
        private readonly CarRentalContext _context;
        private readonly IAccountRepository _accountRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRedisService _redisService;
        private readonly ICloudinaryService _cloudinaryService;

        public UserRepositoryImpl(CarRentalContext context,
            IAccountRepository accountRepository,
            IHttpContextAccessor httpContextAccessor, 
            IRedisService redisService,
            ICloudinaryService cloudinaryService
            )
        {
            _context = context;
            _accountRepository = accountRepository;
            _httpContextAccessor = httpContextAccessor;
            _redisService = redisService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<UserProfile?> GetById(Guid id)
        {
            return await _context.UserProfiles
                .Include(x => x.IdNavigation)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<UserProfile?> UpdateUserProfile(Guid id, UserUpdateDTO dto)
        {
            // Include the Account navigation property
            var user = await _context.UserProfiles
                .Include(u => u.IdNavigation)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) throw new UserNotFoundException();

            // Update UserProfile fields
            user.FullName = dto.FullName;
            user.Dob = dto.Dob;
            user.PhoneNumber = dto.PhoneNumber;
            user.NationalId = dto.NationalId;
            //user.DrivingLicenseUri = dto.DrivingLicenseUri;
            user.HouseNumberStreet = dto.HouseNumberStreet;
            user.Ward = dto.Ward;
            user.District = dto.District;
            user.CityProvince = dto.CityProvince;

            // Update Account email if it's provided and different from current
            if (!string.IsNullOrEmpty(dto.Email) && user.IdNavigation.Email != dto.Email)
            {
                // Check if new email already exists in database
                if (await _context.Accounts.AnyAsync(a => a.Email == dto.Email && a.Id != id))
                {
                    throw new EmailExistException();
                }

                user.IdNavigation.Email = dto.Email;
                user.IdNavigation.UpdatedAt = DateTime.UtcNow;
            }

            // Handle file upload
            if (dto.DrivingLicenseUri != null && dto.DrivingLicenseUri.Length > 0)
            {
                var url = await _cloudinaryService.UploadImageAsync(dto.DrivingLicenseUri, "CarRental/Documents");
                user.DrivingLicenseUri = url;
            }


            await _context.SaveChangesAsync();
            return user;
        }

        private string GetPublicIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                // Find the index of the "upload" segment
                var uploadIndex = Array.IndexOf(segments, "upload");
                if (uploadIndex == -1 || uploadIndex + 2 >= segments.Length)
                    return string.Empty;

                // Everything after upload and version is the public ID (joined if in folders)
                var publicIdSegments = segments.Skip(uploadIndex + 2).ToArray(); // skip "v123456789"
                var publicIdWithExt = string.Join("/", publicIdSegments); // supports folders
                var publicId = Path.ChangeExtension(publicIdWithExt, null); // remove file extension

                return publicId;
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<bool> ChangePassword(Guid id, ChangePasswordDTO dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return false;
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return false;
            }

            // Verify current password (you should hash and compare)
            if (account.Password != dto.CurrentPassword) // In real app, use proper password hashing
            {
                return false;
            }

            account.Password = dto.NewPassword; // In real app, hash the new password
            account.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Register(RegisterDTO dto)
        {
            // Check if email already exists
            if (await _context.Accounts.AnyAsync(x => x.Email == dto.Email))
            {
                return false;
            }

            // Create new account
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Password = dto.Password, // Note: In production, you should hash the password
                IsActive = true,
                IsEmailVerified = false, // Set to true if you have email verification
                CreatedAt = DateTime.UtcNow,
                RoleId = dto.RoleId
            };

            // Create user profile
            var userProfile = new UserProfile
            {
                Id = account.Id,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                IdNavigation = account
            };

            // Create wallet for the user
            var wallet = new Wallet
            {
                Id = account.Id,
                Balance = 0,
                IdNavigation = account
            };

            await _context.Accounts.AddAsync(account);
            await _context.UserProfiles.AddAsync(userProfile);
            await _context.Wallets.AddAsync(wallet);

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<string?> GetUserProfileFullNameByAccountId(Guid accountId)
        {
            return await _context.UserProfiles
                                 .Where(p => p.Id == accountId)
                                 .Select(p => p.FullName)
                                 .FirstOrDefaultAsync();
        }

        public async Task ResetPassword(ChangePasswordDTO dto)
        {
            var emailClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email);
            var jtiClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Jti);
            if (jtiClaim == null)
            {
                throw new InvalidOperationException("JWT ID not found in the token.");
            }
            if (emailClaim == null)
            {
                throw new InvalidOperationException("Email not found in the token.");
            }

            var email = emailClaim.Value;

            var jti = jtiClaim.Value;

            var storedJTI = await _redisService.GetTokenAsync("Forgot_Password_JTI", email);

            if (storedJTI == null || storedJTI != jti)
            {
                throw new InvalidOperationException("Invalid or expired reset password request.");
            }

            var account = await _accountRepository.getAccountByEmailWithRole(email);
            if (account == null)
            {
                throw new InvalidOperationException("Account not found.");
            }

            account.Password = dto.NewPassword;
            account.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _redisService.DeleteTokenAsync("Forgot_Password_JTI", email);
        }
    }


}

