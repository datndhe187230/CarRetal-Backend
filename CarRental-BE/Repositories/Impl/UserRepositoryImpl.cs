using CarRental_BE.Data;
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
                .Include(x => x.IdNavigation) // nếu cần lấy thông tin account liên quan
                .FirstOrDefaultAsync(x => x.Id == id);
        }

         
    }
}
