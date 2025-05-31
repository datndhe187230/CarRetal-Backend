using CarRental_BE.Data;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarRental_BE.Repositories.Impl
{
    public class UserRepositoryImpl : IUserRepository
    {
        private readonly CarRentalContext _context;

        public UserRepositoryImpl(CarRentalContext context)
        {
            _context = context;
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
    }


}
   
