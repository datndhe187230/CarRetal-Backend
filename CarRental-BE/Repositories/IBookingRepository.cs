using CarRental_BE.Models.DTO;
using CarRental_BE.Models.NewEntities;
using CarRental_BE.Models.VO.Statistic;
using Microsoft.EntityFrameworkCore.Storage;

namespace CarRental_BE.Repositories;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllBookingsAsync();
    Task<List<Booking>> GetBookingsByAccountIdAsync(Guid accountId);
    Task<(List<Booking>, int)> GetBookingsWithPagingAsync(int page, int pageSize);
    Task<Booking?> GetByBookingNumberAsync(string bookingNumber);
    Task UpdateAsync(Booking booking);
    Task<Booking?> GetBookingByBookingNumberAsync(string id);
    Task<Booking?> UpdateBookingAsync(string bookingNumber, BookingEditDTO bookingDto);
    Task<IEnumerable<Booking>> GetRecentBookingsAsync(int count = 10);
    Task<int> GetTotalBookingsCountAsync();
    Task<int> GetActiveBookingsCountAsync();
    Task<decimal> GetTotalRevenueAsync();
    Task<IEnumerable<MonthlyRevenueVO>> GetMonthlyRevenueAsync(int year);
    Task<IEnumerable<TopBookedVehicleVO>> GetTopBookedVehiclesAsync(int count = 5);
    Task<IEnumerable<BookingStatusCountVO>> GetBookingStatusCountsAsync();
    Task<int> GetNextBookingSequenceForDateAsync(string datePart);
    Task<Booking?> CreateBookingAsync(Booking newBooking);
    Task<List<Booking>> GetBookingsByCarId(Guid carId);
    Task<bool> UpdateBookingStatusAsync(string bookingNumber, string newStatus);
    Task<List<Booking>> GetBookingsByCarIdAsync(Guid carId);
    Task<IDbContextTransaction> BeginTransactionAsync();
    // Owner-specific dashboard statistics
    Task<decimal> GetOwnerTotalRevenueAsync(Guid ownerAccountId);
    Task<int> GetOwnerActiveBookingsCountAsync(Guid ownerAccountId);
    Task<int> GetOwnerTotalCustomersCountAsync(Guid ownerAccountId);
    Task<IEnumerable<MonthlyRevenueVO>> GetOwnerMonthlyRevenueAsync(Guid ownerAccountId, int year);
    Task<decimal> GetOwnerFleetUtilizationAsync(Guid ownerAccountId);
    Task<IQueryable<Booking>> GetAllBookingsByCarOwnerAsync(Guid ownerAccountId);
    // New optimized query for owner bookings list with filters and paging
    Task<(List<Booking> Items, int TotalCount)> GetOwnerBookingsFilteredAsync(Guid ownerAccountId, CarOwnerBookingListDTO query);
}
