using CarRental_BE.Data;
using CarRental_BE.Exceptions;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.Enum;
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
            if (booking.Status != BookingStatusEnum.PendingDeposit.ToString() &&
                booking.Status != BookingStatusEnum.Confirmed.ToString())
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
    }
}
