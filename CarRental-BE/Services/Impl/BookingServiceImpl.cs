using CarRental_BE.Models.Entities;
using CarRental_BE.Models.Mapper;
using CarRental_BE.Models.VO;
using CarRental_BE.Repositories;
using CarRental_BE.Repositories.Impl;
using CarRental_BE.Services;

public class BookingServiceImpl : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICarRepository _carRepository;
    private readonly IEmailService _emailService;

    public BookingServiceImpl(
        IBookingRepository bookingRepository,
        IAccountRepository accountRepository,
   
        ICarRepository carRepository,
    
        IEmailService emailService)
    {
        _bookingRepository = bookingRepository;
        _accountRepository = accountRepository;
        _carRepository = carRepository;
        _emailService = emailService;
    }

    public async Task<List<BookingVO>> GetAllBookingsAsync()
    {
        var bookingEntities = await _bookingRepository.GetAllBookingsAsync();
        return bookingEntities.Select(BookingMapper.ToBookingVO).ToList();
    }
    public async Task<List<BookingVO>> GetBookingsByAccountIdAsync(Guid accountId)
    {
        var bookingEntities = await _bookingRepository.GetBookingsByAccountIdAsync(accountId);
        return bookingEntities.Select(BookingMapper.ToBookingVO).ToList();
    }
    public async Task<(List<BookingVO>, int)> GetBookingsWithPagingAsync(int page, int pageSize)
    {
        var (entities, totalCount) = await _bookingRepository.GetBookingsWithPagingAsync(page, pageSize);
        var voList = entities.Select(BookingMapper.ToBookingVO).ToList();
        return (voList, totalCount);
    }
    public async Task<(bool Success, string Message)> CancelBookingAsync(string bookingNumber)
    {
        // Lấy thông tin booking từ repository
        var booking = await _bookingRepository.GetByBookingNumberAsync(bookingNumber);
        if (booking == null || booking.Status?.ToLower() == "cancelled")
            return (false, "Invalid or already cancelled booking");

        // Cập nhật trạng thái huỷ
        booking.Status = "cancelled";
        await _bookingRepository.UpdateAsync(booking);

        // Hoàn tiền đặt cọc vào ví khách hàng
        if (booking.AccountId.HasValue)
        {
            var customer = await _accountRepository.GetByIdAsync(booking.AccountId.Value);
            if (customer?.Wallet != null && booking.Deposit.HasValue)
            {
                customer.Wallet.Balance += booking.Deposit.Value;
                await _accountRepository.UpdateAsync(customer);
            }
        }

        // Gửi email thông báo tới chủ xe
        var car = await _carRepository.GetByIdAsync(booking.CarId);
        if (car != null)
        {
            var owner = await _accountRepository.GetByIdAsync(car.AccountId);
            if (owner != null)
            {
                string subject = "Booking Cancelled";
                string body = GenerateCancelBookingEmailContent(booking, owner); // bạn có thể tạo template email riêng
                await _emailService.SendEmailAsync(owner.Email, subject, body);
            }
        }

        return (true, "Cancelled successfully");
    }
    private string GenerateCancelBookingEmailContent(Booking booking, Account owner)
    {
        return $@"
        <h3>Booking Cancelled</h3>
        <p>Hello {owner.Email},</p>
        <p>The booking <strong>{booking.BookingNumber}</strong> has been cancelled by the customer.</p>
        <p>Please check your dashboard for more details.</p>";
    }

}
