using CarRental_BE.Data;
using CarRental_BE.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarRental_BE.Repositories.Impl
{
    public class BookingRepositoryImpl : IBookingRepository
    {
        private readonly CarRentalContext _context;

        public BookingRepositoryImpl(CarRentalContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                                 .Include(b => b.Car)
                                 .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsByAccountIdAsync(Guid accountId)
        {
            return await _context.Bookings
                                 .Include(b => b.Car)
                                 .Where(b => b.AccountId == accountId)
                                 .ToListAsync();
        }

        public async Task<Booking?> GetByBookingNumberAsync(string bookingNumber) => await _context.Bookings
                                 .Include(b => b.Car)
                                 .FirstOrDefaultAsync(b => b.BookingNumber == bookingNumber);
        public async Task<(List<Booking>, int)> GetBookingsWithPagingAsync(int page, int pageSize)
        {
            var query = _context.Bookings
                                .Include(b => b.Car)
                                .Include(b => b.Account)
                                .OrderByDescending(b => b.CreatedAt);

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }


    }
}
