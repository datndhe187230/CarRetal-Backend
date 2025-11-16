using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.Booking;

public interface IBookingService
{
    Task<List<BookingVO>> GetBookingsByAccountIdAsync(Guid accountId);
    Task<(List<BookingVO>, int)> GetBookingsWithPagingAsync(int page, int pageSize);
    Task<List<BookingVO>> GetAllBookingsAsync(); 
    Task<BookingDetailVO?> GetBookingByBookingNumberAsync(string id);
    Task<BookingDetailVO?> UpdateBookingAsync(string bookingNumber, BookingEditDTO bookingDto);
    Task<BookingVO?> CreateBookingAsync(Guid userId, BookingCreateDTO bookingCreateDto);
    Task<OccupiedDateRange[]> GetOccupiedDatesByCarId(Guid carId);
    Task<BookingDetailVO?> GetBookingInformationByCarId(Guid carId);
    Task<List<BookingVO>> GetBookingsByAccountIdAsync(Guid accountId, BookingQueryDto queryDto); // Thêm method mới

    // status flow operations
    Task<(bool Success, string Message)> ConfirmDepositAsync(string bookingNumber);
    Task<(bool Success, string Message)> ConfirmPickupAsync(string bookingNumber);
    Task<(bool Success, string Message)> ConfirmBookingAsync(string bookingNumber);
    Task<(bool Success, string Message)> RequestReturnAsync(string bookingNumber);
    Task<(bool Success, string Message)> AcceptReturnAsync(string bookingNumber, string? note = null, string? pictureUrl = null, decimal? chargesCents = null);
    Task<(bool Success, string Message)> RejectReturnAsync(string bookingNumber, string? note = null, string? pictureUrl = null);
    Task<(bool Success, string Message)> CustomerCancelAsync(string bookingNumber, string? reason = null, string? pictureUrl = null);
    Task<(bool Success, string Message)> OwnerCancelAsync(string bookingNumber, string? reason = null, string? pictureUrl = null);

    // new: summary for front-end
    Task<BookingSummaryVO?> GetBookingSummaryAsync(string bookingNumber);
}

