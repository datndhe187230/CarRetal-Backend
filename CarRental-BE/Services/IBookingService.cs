using CarRental_BE.Models.VO;

public interface IBookingService
{
    Task<List<BookingVO>> GetBookingsByAccountIdAsync(Guid accountId);
    Task<(List<BookingVO>, int)> GetBookingsWithPagingAsync(int page, int pageSize);
    Task<List<BookingVO>> GetAllBookingsAsync(); // Thêm dòng này
}
