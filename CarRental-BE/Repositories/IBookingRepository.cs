using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.VO.Statistic;

namespace CarRental_BE.Repositories;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllBookingsAsync();
    Task<List<Booking>> GetBookingsByAccountIdAsync(Guid accountId);
    Task<(List<Booking>, int)> GetBookingsWithPagingAsync(int page, int pageSize);
    Task<Booking?> GetByBookingNumberAsync(string bookingNumber);
    Task UpdateAsync(Booking booking);
    Task<Booking?> GetBookingByBookingIdAsync(string id);
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
}