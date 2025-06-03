using CarRental_BE.Data;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using CarRental_BE.Services;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CarRental_BE.Repositories.Impl
{
    public class UserRepositoryImpl : IUserRepository
    {
        private readonly CarRentalContext _context;
        private readonly IAccountRepository _accountRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRedisService _redisService;

        public UserRepositoryImpl(CarRentalContext context, IAccountRepository accountRepository, IHttpContextAccessor httpContextAccessor, IRedisService redisService)
        {
            _context = context;
            _accountRepository = accountRepository;
            _httpContextAccessor = httpContextAccessor;
            _redisService = redisService;
        }

        public async Task<UserProfile?> GetById(Guid id)
        {
            return await _context.UserProfiles
                .Include(x => x.IdNavigation)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<UserProfile?> UpdateUserProfile(Guid id, UserUpdateDTO dto)
        {
            var user = await _context.UserProfiles.FindAsync(id);
            if (user == null) return null;

            user.FullName = dto.FullName;
            user.Dob = dto.Dob;
            user.PhoneNumber = dto.PhoneNumber;
            user.NationalId = dto.NationalId;
            user.DrivingLicenseUri = dto.DrivingLicenseUri;
            user.HouseNumberStreet = dto.HouseNumberStreet;
            user.Ward = dto.Ward;
            user.District = dto.District;
            user.CityProvince = dto.CityProvince;

            await _context.SaveChangesAsync(); // Không cần gọi Update() vì EF Core đang tracking
            return user;
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

