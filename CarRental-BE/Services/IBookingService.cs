using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO;
using CarRental_BE.Models.VO.User;

public interface IBookingService
{
    Task<List<BookingVO>> GetBookingsByAccountIdAsync(Guid accountId);
    Task<(List<BookingVO>, int)> GetBookingsWithPagingAsync(int page, int pageSize);
    
    Task<(bool Success, string Message)> CancelBookingAsync(string bookingNumber);

    Task<List<BookingVO>> GetAllBookingsAsync(); 
    Task<BookingDetailVO?> GetBookingByBookingIdAsync(string id);
    Task<BookingDetailVO?> UpdateBookingAsync(string bookingNumber, BookingEditDTO bookingDto);
    Task<(bool Success, string Message)> ConfirmPickupAsync(string bookingNumber);
    Task<(bool Success, string Message)> ReturnCarAsync(string bookingNumber);

}
