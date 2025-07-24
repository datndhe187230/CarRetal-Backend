using CarRental_BE.Data;
using CarRental_BE.Exceptions;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.Enum;
using CarRental_BE.Models.VO.Statistic;
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
                                 .Include(b => b.Account).ThenInclude(a => a.Wallet)

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


        public async Task<Booking?> GetBookingByBookingIdAsync(string id)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Account)
                    .ThenInclude(a => a.UserProfile)
                .FirstOrDefaultAsync(b => b.BookingNumber == id);
        }

        public async Task<Booking?> UpdateBookingAsync(string bookingNumber, BookingEditDTO bookingDto)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingNumber == bookingNumber);

            if (booking == null)
                 throw new BookingNotFoundException(bookingNumber);

            // Check if booking status allows editing
            if (!booking.Status.Equals(BookingStatusEnum.pending_deposit.ToString(), StringComparison.OrdinalIgnoreCase) &&
                !booking.Status.Equals(BookingStatusEnum.confirmed.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                throw new BookingEditException(booking.Status);
            }

            // Update only the allowed fields
            booking.DriverFullName = bookingDto.DriverFullName ?? booking.DriverFullName;
            booking.DriverDob = bookingDto.DriverDob ?? booking.DriverDob;
            booking.DriverEmail = bookingDto.DriverEmail ?? booking.DriverEmail;
            booking.DriverPhoneNumber = bookingDto.DriverPhoneNumber ?? booking.DriverPhoneNumber;
            booking.DriverNationalId = bookingDto.DriverNationalId ?? booking.DriverNationalId;
            booking.DriverHouseNumberStreet = bookingDto.DriverHouseNumberStreet ?? booking.DriverHouseNumberStreet;
            booking.DriverWard = bookingDto.DriverWard ?? booking.DriverWard;
            booking.DriverDistrict = bookingDto.DriverDistrict ?? booking.DriverDistrict;
            booking.DriverCityProvince = bookingDto.DriverCityProvince ?? booking.DriverCityProvince;
            booking.DriverDrivingLicenseUri = bookingDto.DriverDrivingLicenseUri ?? booking.DriverDrivingLicenseUri;

            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<IEnumerable<Booking>> GetRecentBookingsAsync(int count = 10)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Account)
                .OrderByDescending(b => b.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        //Dashboard statistics methods
        public async Task<int> GetTotalBookingsCountAsync()
        {
            return await _context.Bookings.CountAsync();
        }

        public async Task<int> GetActiveBookingsCountAsync()
        {
            return await _context.Bookings
                .CountAsync(b => b.Status == "Active" || b.Status == "Confirmed");
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Bookings
                .Where(b => b.Status == "Completed")
                .SumAsync(b => (decimal)(b.BasePrice ?? 0));
        }

        public async Task<IEnumerable<MonthlyRevenueVO>> GetMonthlyRevenueAsync(int year)
        {
            return await _context.Bookings
                .Where(b => b.CreatedAt.HasValue && b.CreatedAt.Value.Year == year && b.Status == "Completed")
                .GroupBy(b => b.CreatedAt!.Value.Month) // Use the null-forgiving operator '!' to suppress the warning
                .Select(g => new MonthlyRevenueVO
                {
                    Month = g.Key.ToString(),
                    Total = g.Sum(b => (decimal)(b.BasePrice ?? 0))
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TopBookedVehicleVO>> GetTopBookedVehiclesAsync(int count = 5)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .GroupBy(b => new
                {
                    b.CarId,
                    b.Car.Brand,
                    b.Car.Model,
                    b.Car.ProductionYear,
                    b.Car.Status
                })
                .Select(g => new TopBookedVehicleVO
                {
                    CarId = g.Key.CarId,
                    Brand = g.Key.Brand,
                    Model = g.Key.Model,
                    Year = g.Key.ProductionYear,
                    TotalBookings = g.Count(),
                    Revenue = g.Sum(b => (decimal)(b.BasePrice ?? 0)),
                    UtilizationRate = (decimal)g.Count() / 30 * 100, // Assuming 30 days calculation
                    Status = g.Key.Status,
                    Trend = "+12%"
                })
                .OrderByDescending(x => x.TotalBookings)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingStatusCountVO>> GetBookingStatusCountsAsync()
        {
            return await _context.Bookings
                .GroupBy(b => b.Status)
                .Select(g => new BookingStatusCountVO
                {
                    Status = g.Key ?? "Unknown",
                    Count = g.Count()
                })
                .ToListAsync();
        }

        public async Task<int> GetNextBookingSequenceForDateAsync(string datePart)
        {
            return await _context.Bookings
                .Where(b => b.BookingNumber.StartsWith(datePart)).CountAsync() + 1;
        }

        public async Task<Booking?> CreateBookingAsync(Booking newBooking)
        {
            _context.Bookings.Add(newBooking);
            await _context.SaveChangesAsync();
            return newBooking;
        }

        public async Task<List<Booking>> GetBookingsByCarId(Guid carId)
        {
            return await _context.Bookings.Where(b => b.PickUpTime > DateTime.Today).Where(b => b.CarId == carId).ToListAsync();
        }
    }
}
