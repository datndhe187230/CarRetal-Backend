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

        public Task<(List<Car> cars, int totalCount)> GetAccountId(
            Guid accountId, 
            int pageNumber, 
            int pageSize)
        {
            var query = _context.Cars.Where(c => c.AccountId == accountId);

            var totalCount = query.Count();
            var cars = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        return Task.FromResult((cars: cars.Result, totalCount: totalCount));

        }
    }
}
