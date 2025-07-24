namespace CarRental_BE.Models.Entities;

using CarRental_BE.Exceptions;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Enum;
using CarRental_BE.Models.Mapper;
using CarRental_BE.Models.VO;
using CarRental_BE.Models.VO.Booking;
using CarRental_BE.Models.VO.User;
using CarRental_BE.Repositories;
using CarRental_BE.Repositories.Impl;
using CarRental_BE.Services;
using System.Threading.Tasks;

public class BookingServiceImpl : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICarRepository _carRepository;
    private readonly IEmailService _emailService;
    private readonly IWalletService _walletService;

    public BookingServiceImpl(
        IBookingRepository bookingRepository,
        IAccountRepository accountRepository,
        ICarRepository carRepository,
        IEmailService emailService,
        IWalletService walletService
        )
    {
        _bookingRepository = bookingRepository;
        _accountRepository = accountRepository;
        _carRepository = carRepository;
        _emailService = emailService;
        _walletService = walletService
        ;
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
                string body = GenerateCancelBookingEmailContent(booking, owner);
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
    public async Task<BookingDetailVO?> GetBookingByBookingIdAsync(string id)
    {
        var entity = await _bookingRepository.GetBookingByBookingIdAsync(id);
        return entity != null ? BookingMapper.ToBookingDetailVO(entity) : null;
    }


    public async Task<BookingDetailVO?> UpdateBookingAsync(string bookingNumber, BookingEditDTO bookingDto)
    {
        var updatedBooking = await _bookingRepository.UpdateBookingAsync(bookingNumber, bookingDto);
        return updatedBooking != null ? BookingMapper.ToBookingDetailVO(updatedBooking) : null;
    }

    public async Task<(bool Success, string Message)> ConfirmPickupAsync(string bookingNumber)
    {
        var booking = await _bookingRepository.GetByBookingNumberAsync(bookingNumber);
        if (booking == null)
            return (false, "Booking not found");

        if (booking.Status?.ToLower() != "confirmed")
            return (false, "Only confirmed bookings can be picked up");

        booking.Status = "in_progress";
        await _bookingRepository.UpdateAsync(booking);

        return (true, "Booking marked as in progress");
    }
    public async Task<(bool Success, string Message)> ReturnCarAsync(string bookingNumber)
    {
        var booking = await _bookingRepository.GetByBookingNumberAsync(bookingNumber);
        if (booking == null)
            return (false, "Booking not found");

        if (booking.Status?.ToLower() != "in_progress")
            return (false, "Only in-progress bookings can be returned");

        var pickup = booking.PickUpTime ?? DateTime.UtcNow;
        var dropoff = booking.DropOffTime ?? DateTime.UtcNow;
        var totalAmount = booking.BasePrice;
        var deposit = booking.Deposit ?? 0;

        var customer = await _accountRepository.GetByIdAsync(booking.AccountId!.Value);
        if (customer?.Wallet == null)
            return (false, "Customer wallet not found");

        if (totalAmount > deposit)
        {
            var diff = totalAmount - deposit;
            if (customer.Wallet.Balance < diff)
            {
                booking.Status = "pending_payment";
                await _bookingRepository.UpdateAsync(booking);
                return (false, "ME012: Your wallet doesn’t have enough balance. Please top-up your wallet and try again.");
            }

            customer.Wallet.Balance -= (long)diff;
            await _accountRepository.UpdateAsync(customer);
        }
        else if (deposit > totalAmount)
        {
            var refund = deposit - totalAmount;
            customer.Wallet.Balance += (long)refund;
            await _accountRepository.UpdateAsync(customer);
        }

        booking.Status = "completed";
        await _bookingRepository.UpdateAsync(booking);

        var car = booking.Car ?? await _carRepository.GetByIdAsync(booking.CarId);
        var owner = car != null ? await _accountRepository.GetByIdAsync(car.AccountId) : null;

        if (owner != null)
        {
            string subject = "Car Returned";
            string body = $@"
        <h3>Car Returned</h3>
        <p>Hello {owner.Email},</p>
        <p>Booking <strong>{booking.BookingNumber}</strong> has been returned successfully by the customer.</p>
        <p>Please check your dashboard for more details.</p>";
            await _emailService.SendEmailAsync(owner.Email, subject, body);
        }

        return (true, "Return completed successfully");
    }

    public async Task<BookingVO?> CreateBookingAsync(Guid userId, BookingCreateDTO bookingCreateDto)
    {
        Car? car = await _carRepository.GetByIdAsync(bookingCreateDto.CarId);
        if (car == null)
        {
            throw new ArgumentException("Car not found.");
        }

        if (bookingCreateDto.PaymentType == "cash")
        {
            string bookingNumber = await GenerateBookingNumberAsync();
            decimal basePrice = car.BasePrice * bookingCreateDto.RentalDays;
            Booking newBooking = BookingMapper.ToBookingEntity(bookingCreateDto, userId, BookingStatusEnum.pending_deposit, basePrice, bookingNumber);

            newBooking = await _bookingRepository.CreateBookingAsync(newBooking);

            if (newBooking == null)
            {
                throw new InvalidOperationException("Failed to create booking.");
            }

            //TODO: Send confirmation email to user

            return BookingMapper.ToBookingVO(newBooking);

        }
        else if (bookingCreateDto.PaymentType == "wallet")
        {
            await CheckWallet(userId, bookingCreateDto); // Add 'await' here

            string bookingNumber = await GenerateBookingNumberAsync();

            //Deduct balance from customer wallet
            WithdrawDTO withdrawDTO = new WithdrawDTO
            {
                Amount = (long)bookingCreateDto.Deposit,
                Message = "Booking deposit"
            };

            await _walletService.WithdrawMoney(userId, withdrawDTO);


            //Add deposite to car owner balance
            TopupDTO topupDTO = new TopupDTO
            {
                Amount = (long)bookingCreateDto.Deposit,
                Message = "Booking deposite paid"
            };
            await _walletService.TopupMoney(car.AccountId, topupDTO);

            decimal basePrice = car.BasePrice * bookingCreateDto.RentalDays;

            if (((double)bookingCreateDto.Deposit) < ((double)basePrice * 0.3))
            {
                throw new InvalidOperationException("Deposite amount must be greater than 30% of total");
            }

            Booking newBooking = BookingMapper.ToBookingEntity(bookingCreateDto, userId, BookingStatusEnum.confirmed, basePrice, bookingNumber);

            newBooking = await _bookingRepository.CreateBookingAsync(newBooking);
            //TODO: Send confirmation email to user and car owner

            return BookingMapper.ToBookingVO(newBooking);
        }
        else
        {
            throw new ArgumentException("Invalid payment type. Must be 'cash' or 'wallet'.");
        }

        return null; // Adjust return as needed based on your logic  
    }

    private async Task CheckWallet(Guid userId, BookingCreateDTO bookingCreateDto)
    {
        decimal walletBalance = (await _walletService.GetWalletBalance(userId)).Balance;
        if (bookingCreateDto.Deposit < 0)
        {
            throw new ArgumentException("Deposited amount cannot be negative for wallet payments.");
        }
        else if (walletBalance < bookingCreateDto.Deposit)
        {
            throw new ArgumentException("Insufficient wallet balance for the booking.");
        }
    }

    private async Task<string> GenerateBookingNumberAsync()
    {
        // Get today's date in YYYYMMdd format
        string datePart = DateTime.UtcNow.ToString("yyyyMMdd");

        // Get the next sequence number from the database for today
        int sequence = await _bookingRepository.GetNextBookingSequenceForDateAsync(datePart);

        // Combine to form booking number
        return $"{datePart}-{sequence}";
    }

    public async Task<OccupiedDateRange[]> GetOccupiedDatesByCarId(Guid carId)
    {
        List<Booking> bookings = (List<Booking>)await _bookingRepository.GetBookingsByCarId(carId);
        return bookings.Select(b => new OccupiedDateRange
        {
            Start = (DateTime)b.PickUpTime,
            End = (DateTime)b.DropOffTime,
        }).ToArray();
    }
}
