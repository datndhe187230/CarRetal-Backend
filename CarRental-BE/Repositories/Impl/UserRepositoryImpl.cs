using CarRental_BE.Exceptions;
using CarRental_BE.Models.DTO;
using CarRental_BE.Services;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InvalidOperationException = CarRental_BE.Exceptions.InvalidOperationException;
using CarRental_BE.Helpers;
using CarRental_BE.Models.NewEntities;
using CarRental_BE.Data;

namespace CarRental_BE.Repositories.Impl
{
    public class UserRepositoryImpl : IUserRepository
    {
        private readonly CarRentalContext _context;
        private readonly IAccountRepository _accountRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRedisService _redisService;
        private readonly ICloudinaryService _cloudinaryService;

        public UserRepositoryImpl(CarRentalContext context, IAccountRepository accountRepository, IHttpContextAccessor httpContextAccessor, IRedisService redisService, ICloudinaryService cloudinaryService)
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
                .Include(x => x.Address)
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.AccountId == id);
        }

        public async Task<UserProfile?> UpdateUserProfile(Guid id, UserUpdateDTO dto)
        {
            var user = await _context.UserProfiles
                .Include(u => u.Account)
                .FirstOrDefaultAsync(u => u.AccountId == id);

            if (user == null) throw new UserNotFoundException();

            user.FullName = dto.FullName;
            user.Dob = dto.Dob;
            user.PhoneNumber = dto.PhoneNumber;
            user.NationalId = dto.NationalId;

            if (!string.IsNullOrEmpty(dto.Email) && user.Account.Email != dto.Email)
            {
                if (await _context.Accounts.AnyAsync(a => a.Email == dto.Email && a.AccountId != id)) throw new EmailExistException();
                user.Account.Email = dto.Email;
                user.Account.UpdatedAt = DateTime.UtcNow;
            }

            if (dto.DrivingLicenseUri != null && dto.DrivingLicenseUri.Length > 0)
            {
                var url = await _cloudinaryService.UploadImageAsync(dto.DrivingLicenseUri, "CarRental/Documents");
                user.DrivingLicenseUri = url;
            }

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ChangePassword(Guid id, ChangePasswordDTO dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword) return false;
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return false;
            if (!PasswordHelper.VerifyPassword(dto.CurrentPassword, account.PasswordHash)) return false;
            account.PasswordHash = PasswordHelper.HashPassword(dto.NewPassword);
            account.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Register(RegisterDTO dto)
        {
            if (await _context.Accounts.AnyAsync(x => x.Email == dto.Email)) return false;
            var account = new Account
            {
                AccountId = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                IsActive = true,
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow,
                RoleId = dto.RoleId
            };
            var userProfile = new UserProfile
            {
                AccountId = account.AccountId,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Account = account
            };
            var wallet = new Wallet
            {
                AccountId = account.AccountId,
                BalanceCents = 0m,
                LockedCents = 0m,
                Account = account,
                UpdatedAt = DateTime.UtcNow
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
                                 .Where(p => p.AccountId == accountId)
                                 .Select(p => p.FullName)
                                 .FirstOrDefaultAsync();
        }

        public async Task ResetPassword(ChangePasswordDTO dto)
        {
            var emailClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email);
            var jtiClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Jti);
            if (jtiClaim == null) throw new InvalidOperationException("JWT ID not found in the token.");
            if (emailClaim == null) throw new InvalidOperationException("Email not found in the token.");
            var email = emailClaim.Value;
            var jti = jtiClaim.Value;
            var storedJTI = await _redisService.GetTokenAsync("Forgot_Password_JTI", email);
            if (storedJTI == null || storedJTI != jti) throw new InvalidOperationException("Invalid or expired reset password request.");
            var account = await _accountRepository.getAccountByEmailWithRole(email);
            if (account == null) throw new InvalidOperationException("Account not found.");
            account.PasswordHash = PasswordHelper.HashPassword(dto.NewPassword);
            account.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await _redisService.DeleteTokenAsync("Forgot_Password_JTI", email);
        }
    }
}

