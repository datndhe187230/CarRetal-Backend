using CarRental_BE.Models.Entities;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllBookingsAsync();
    Task<List<Booking>> GetBookingsByAccountIdAsync(Guid accountId);
    Task<(List<Booking>, int)> GetBookingsWithPagingAsync(int page, int pageSize);
    Task<Booking> GetByBookingNumberAsync(string bookingNumber);
    Task UpdateAsync(Booking booking);


}
