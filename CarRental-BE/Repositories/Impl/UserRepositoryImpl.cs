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

    }


}
   
