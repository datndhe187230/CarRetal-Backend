using CarRental_BE.Data;
using CarRental_BE.Exceptions;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.Enum;
using CarRental_BE.Models.VO.Statistic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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
                                 .OrderByDescending(b => b.CreatedAt)
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
            return await _context.Bookings.Where(b => b.Status !=BookingStatusEnum.cancelled.ToString() && b.Status != BookingStatusEnum.confirmed.ToString() && b.PickUpTime > DateTime.Today).Where(b => b.CarId == carId).ToListAsync();
        }
        public async Task<bool> UpdateBookingStatusAsync(string bookingNumber, string newStatus)
        {
            var booking = await _context.Bookings
                                       .FirstOrDefaultAsync(b => b.BookingNumber == bookingNumber);
            if (booking == null)
                return false;

            booking.Status = newStatus;
            booking.UpdatedAt = DateTime.UtcNow;

            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Booking>> GetBookingsByCarIdAsync(Guid carId)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Where(b =>
                    b.CarId == carId &&
                    b.Status != BookingStatusEnum.cancelled.ToString() &&
                    b.Status != BookingStatusEnum.completed.ToString())
                .ToListAsync();
        }


        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        // Owner dashboard implementations
        public async Task<decimal> GetOwnerTotalRevenueAsync(Guid ownerAccountId)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.Car.AccountId == ownerAccountId && b.Status == BookingStatusEnum.completed.ToString())
                .SumAsync(b => (decimal)(b.BasePrice ?? 0));
        }

        public async Task<int> GetOwnerActiveBookingsCountAsync(Guid ownerAccountId)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .CountAsync(b => b.Car.AccountId == ownerAccountId && (b.Status == BookingStatusEnum.in_progress.ToString() || b.Status == BookingStatusEnum.confirmed.ToString()));
        }

        public async Task<int> GetOwnerTotalCustomersCountAsync(Guid ownerAccountId)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.Car.AccountId == ownerAccountId)
                .Select(b => b.AccountId)
                .Distinct()
                .CountAsync();
        }

        public async Task<IEnumerable<MonthlyRevenueVO>> GetOwnerMonthlyRevenueAsync(Guid ownerAccountId, int year)
        {
            return await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.Car.AccountId == ownerAccountId && b.CreatedAt.HasValue && b.CreatedAt.Value.Year == year && b.Status == BookingStatusEnum.completed.ToString())
                .GroupBy(b => b.CreatedAt!.Value.Month)
                .Select(g => new MonthlyRevenueVO
                {
                    Month = g.Key.ToString(),
                    Total = g.Sum(b => (decimal)(b.BasePrice ?? 0))
                })
                .ToListAsync();
        }

        public async Task<decimal> GetOwnerFleetUtilizationAsync(Guid ownerAccountId)
        {
            // naive approximation: percentage of owner's cars that have at least one active/confirmed booking today
            var ownerCarIds = await _context.Cars.Where(c => c.AccountId == ownerAccountId).Select(c => c.Id).ToListAsync();
            if (!ownerCarIds.Any()) return 0m;
            var activeCars = await _context.Bookings
                .Where(b => ownerCarIds.Contains(b.CarId) && (b.Status == BookingStatusEnum.in_progress.ToString() || b.Status == BookingStatusEnum.confirmed.ToString()))
                .Select(b => b.CarId)
                .Distinct()
                .CountAsync();
            return Math.Round(((decimal)activeCars / ownerCarIds.Count) * 100m, 1);
        }

        public async Task<IQueryable<Booking>> GetAllBookingsByCarOwnerAsync(Guid ownerAccountId)
        {
            return _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Transactions)
                .Where(b => b.Car.AccountId == ownerAccountId)
                .AsNoTracking()
                .AsQueryable();
        }

        public async Task<(List<Booking> Items, int TotalCount)> GetOwnerBookingsFilteredAsync(Guid ownerAccountId, CarOwnerBookingListDTO query)
        {
            var q = _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Transactions)
                .Where(b => b.Car.AccountId == ownerAccountId)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var term = query.Search.Trim().ToLower();
                q = q.Where(b =>
                    (b.BookingNumber != null && b.BookingNumber.ToLower().Contains(term)) ||
                    (b.DriverFullName != null && b.DriverFullName.ToLower().Contains(term)) ||
                    (b.DriverEmail != null && b.DriverEmail.ToLower().Contains(term)) ||
                    (b.DriverPhoneNumber != null && b.DriverPhoneNumber.ToLower().Contains(term)));
            }
            // Car name filter
            if (!string.IsNullOrWhiteSpace(query.CarName))
            {
                var carTerm = query.CarName.Trim().ToLower();
                q = q.Where(b => (b.Car.Brand + " " + b.Car.Model).ToLower().Contains(carTerm) || b.Car.Model.ToLower().Contains(carTerm));
            }
            // Statuses
            if (query.Status != null && query.Status.Any())
            {
                var wanted = query.Status.Select(s => s.Trim().ToLower()).ToList();
                q = q.Where(b => b.Status != null && wanted.Contains(b.Status.ToLower()));
            }
            // Date overlap
            if (query.FromDate.HasValue || query.ToDate.HasValue)
            {
                var from = query.FromDate?.Date ?? DateTime.MinValue.Date;
                var to = (query.ToDate?.Date ?? DateTime.MaxValue.Date).AddDays(1).AddTicks(-1);
                q = q.Where(b => b.PickUpTime.HasValue && b.DropOffTime.HasValue && b.PickUpTime <= to && b.DropOffTime >= from);
            }

            // Sorting
            var sortBy = (query.SortBy ?? string.Empty).ToLower();
            var sortDir = (query.SortDirection ?? "desc").ToLower();
            bool asc = sortDir == "asc";
            IOrderedQueryable<Booking>? ordered = null;
            if (sortBy == "pickupdate")
            {
                ordered = asc ? q.OrderBy(b => b.PickUpTime) : q.OrderByDescending(b => b.PickUpTime);
            }
            else if (sortBy == "returndate")
            {
                ordered = asc ? q.OrderBy(b => b.DropOffTime) : q.OrderByDescending(b => b.DropOffTime);
            }
            else if (sortBy == "totalamount")
            {
                ordered = asc ? q.OrderBy(b => (b.BasePrice ?? 0) + (b.Deposit ?? 0)) : q.OrderByDescending(b => (b.BasePrice ?? 0) + (b.Deposit ?? 0));
            }
            else if (sortBy == "status")
            {
                ordered = asc ? q.OrderBy(b => b.Status) : q.OrderByDescending(b => b.Status);
            }
            else
            {
                ordered = q
                    .OrderBy(b => b.Status == BookingStatusEnum.confirmed.ToString() ? 0
                                    : b.Status == BookingStatusEnum.in_progress.ToString() ? 1
                                    : b.Status == BookingStatusEnum.pending_payment.ToString() ? 2
                                    : b.Status == BookingStatusEnum.pending_deposit.ToString() ? 3
                                    : 4)
                    .ThenByDescending(b => b.PickUpTime);
            }

            int totalCount = await ordered.CountAsync();
            int page = query.Page < 1 ? 1 : query.Page;
            int pageSize = query.PageSize < 1 ? 10 : query.PageSize > 100 ? 100 : query.PageSize;
            var items = await ordered.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, totalCount);
        }
    }
}
