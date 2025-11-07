using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO;
using CarRental_BE.Models.VO.Booking;
using CarRental_BE.Models.VO.Car;
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
    Task<BookingVO?> CreateBookingAsync(Guid userId, BookingCreateDTO bookingCreateDto);
    Task<OccupiedDateRange[]> GetOccupiedDatesByCarId(Guid carId);

    Task<(bool Success, string Message)> ConfirmDepositAsync(string bookingNumber);
    Task<BookingDetailVO?> GetBookingInformationByCarId(Guid carId);


    Task<List<BookingVO>> GetBookingsByAccountIdAsync(Guid accountId, BookingQueryDto queryDto); // Thêm method mới

}

