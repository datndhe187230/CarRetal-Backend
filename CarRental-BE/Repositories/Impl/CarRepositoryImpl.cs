using CarRental_BE.Data;
using CarRental_BE.Models.Entities;
using CarRental_BE.Services;
using Microsoft.EntityFrameworkCore;

namespace CarRental_BE.Repositories.Impl
{
    public class CarRepositoryImpl : ICarRepository
    {
        private readonly CarRentalContext _context;

        public CarRepositoryImpl(CarRentalContext context)
        {
            _context = context;
        }

        public async Task<List<Car>> GetAccountId(Guid accountId)
        {
            return await _context.Cars
                .Where(c => c.AccountId == accountId)
                .ToListAsync();
        }

    }


}
