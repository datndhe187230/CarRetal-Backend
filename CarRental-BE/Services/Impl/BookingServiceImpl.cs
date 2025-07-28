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
        // Get booking info from repository
        var booking = await _bookingRepository.GetByBookingNumberAsync(bookingNumber);
        if (booking == null || booking.Status?.ToLower() == "cancelled")
            return (false, "Invalid or already cancelled booking");

        // If booking is confirmed, revert transaction
        if (booking.Status?.ToLower() == "confirmed")
        {
            // Revert wallet transactions for this booking
            await _walletService.RevertBookingTransactionsAsync(bookingNumber);
        }

        // Update status to cancelled
        booking.Status = "cancelled";
        await _bookingRepository.UpdateAsync(booking);

        // Send email notification to car owner
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

        var totalAmount = booking.BasePrice ?? 0;
        var deposit = booking.Deposit ?? 0;

        var customer = await _accountRepository.GetByIdAsync(booking.AccountId!.Value);
        if (customer?.Wallet == null)
            return (false, "Customer wallet not found");

        // 1. Create transaction for remaining payment or refund
        if (totalAmount > deposit)
        {
            var diff = totalAmount - deposit;
            if (customer.Wallet.Balance < diff)
            {
                booking.Status = "pending_payment";
                await _bookingRepository.UpdateAsync(booking);
                return (false, "ME012: Your wallet doesn’t have enough balance. Please top-up your wallet and try again.");
            }

            // Deduct remaining amount from wallet and create transaction
            customer.Wallet.Balance -= (long)diff;
            await _accountRepository.UpdateAsync(customer);

            var payTransaction = new TransactionDTO
            {
                Amount = (long)diff,
                Message = "Pay remaining rental fee",
                BookingId = booking.BookingNumber,
                CarName = booking.Car != null ? $"{booking.Car.Brand} {booking.Car.Model} {booking.Car.ProductionYear} {booking.Car.LicensePlate}" : "",
                Type = TransactionType.offset_final_payment,
                Status = TransactionStatus.Successful
            };
            await _walletService.WithdrawMoney(customer.Id, payTransaction);
        }
        else if (deposit > totalAmount)
        {
            var refund = deposit - totalAmount;
            customer.Wallet.Balance += (long)refund;
            await _accountRepository.UpdateAsync(customer);

            var refundTransaction = new TransactionDTO
            {
                Amount = (long)refund,
                Message = "Refund excess deposit",
                BookingId = booking.BookingNumber,
                CarName = booking.Car != null ? $"{booking.Car.Brand} {booking.Car.Model} {booking.Car.ProductionYear} {booking.Car.LicensePlate}" : "",
                Type = TransactionType.offset_final_payment,
                Status = TransactionStatus.Successful
            };
            await _walletService.TopupMoney(customer.Id, refundTransaction);
        }

        // 2. Update all pending deposit transactions to successful
        var depositTransactions = booking.Transactions
            .Where(t => (t.Type == TransactionType.pay_deposit.ToString() || t.Type != TransactionType.receive_deposit.ToString()) &&
                        t.Status == TransactionStatus.Processing.ToString())
            .ToList();

        foreach (var transaction in depositTransactions)
        {
            await _walletService.UpdateTransactionStatusAsync(transaction.Id, TransactionStatus.Successful.ToString());
        }

        booking.Status = BookingStatusEnum.completed.ToString();
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
        // Begin a transaction
        using (var transaction = await _bookingRepository.BeginTransactionAsync())
        {
            try
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

                    // TODO: Send confirmation email to user

                    // Commit the transaction for cash payment
                    await transaction.CommitAsync();

                    return BookingMapper.ToBookingVO(newBooking);
                }
                else if (bookingCreateDto.PaymentType == "wallet")
                {
                    await CheckWallet(userId, bookingCreateDto);

                    string bookingNumber = await GenerateBookingNumberAsync();
                    decimal basePrice = car.BasePrice * bookingCreateDto.RentalDays;

                    if (((double)bookingCreateDto.Deposit) < ((double)basePrice * 0.3))
                    {
                        throw new InvalidOperationException("Deposit amount must be greater than 30% of total");
                    }

                    Booking newBooking = BookingMapper.ToBookingEntity(bookingCreateDto, userId, BookingStatusEnum.confirmed, basePrice, bookingNumber);

                    newBooking = await _bookingRepository.CreateBookingAsync(newBooking);
                    if (newBooking == null)
                    {
                        throw new InvalidOperationException("Failed to create booking.");
                    }

                    // Deduct balance from customer wallet
                    TransactionDTO withdrawDTO = new TransactionDTO
                    {
                        Amount = (long)bookingCreateDto.Deposit,
                        Message = "Booking deposit",
                        BookingId = bookingNumber,
                        CarName = $"{car.Brand} {car.Model} {car.ProductionYear} {car.LicensePlate}",
                        Type = TransactionType.pay_deposit,
                        Status = TransactionStatus.Processing
                    };

                    await _walletService.WithdrawMoney(userId, withdrawDTO);

                    // Add 90% deposit to car owner balance
                    TransactionDTO topupDTO = new TransactionDTO
                    {
                        Amount = (long)(bookingCreateDto.Deposit * 0.9m),
                        Message = "Booking deposit paid",
                        BookingId = bookingNumber,
                        CarName = $"{car.Brand} {car.Model} {car.ProductionYear} {car.LicensePlate}",
                        Type = TransactionType.receive_deposit,
                        Status = TransactionStatus.Processing
                    };
                    await _walletService.TopupMoney(car.AccountId, topupDTO);

                    // Add 10% deposit to admin balance
                    TransactionDTO topupAdminDTO = new TransactionDTO
                    {
                        Amount = (long)(bookingCreateDto.Deposit * 0.1m),
                        Message = "Booking deposit commission",
                        BookingId = bookingNumber,
                        CarName = $"{car.Brand} {car.Model} {car.ProductionYear} {car.LicensePlate}",
                        Type = TransactionType.receive_deposit
                    };
                    await _walletService.TopupMoneyAdmin(topupAdminDTO);

                    // TODO: Send confirmation email to user and car owner

                    // Commit the transaction for wallet payment
                    await transaction.CommitAsync();

                    return BookingMapper.ToBookingVO(newBooking);
                }
                else
                {
                    throw new ArgumentException("Invalid payment type. Must be 'cash' or 'wallet'.");
                }
            }
            catch
            {
                // Roll back the transaction on any error
                await transaction.RollbackAsync();
                throw;
            }
        }
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

    public async Task<(bool Success, string Message)> ConfirmDepositAsync(string bookingNumber)
    {
        var booking = await _bookingRepository.GetByBookingNumberAsync(bookingNumber);
        if (booking == null)
            return (false, "Booking not found");

        if (booking.Status != BookingStatusEnum.pending_deposit.ToString())
            return (false, "Only bookings with 'pending_deposit' status can be confirmed");

        var result = await _bookingRepository.UpdateBookingStatusAsync(bookingNumber, BookingStatusEnum.confirmed.ToString());
        if (!result)
            return (false, "Failed to update booking status");

        var car = await _carRepository.GetByIdAsync(booking.CarId);
        if (car != null)
        {
            var owner = await _accountRepository.GetByIdAsync(car.AccountId);
            if (owner != null)
            {
                string subject = "Deposit Confirmed";
                string body = $@"
                <h3>Deposit Confirmed</h3>
                <p>Hello {owner.Email},</p>
                <p>The deposit for booking <strong>{booking.BookingNumber}</strong> has been confirmed.</p>
                <p>Please check your dashboard for more details.</p>";
                await _emailService.SendEmailAsync(owner.Email, subject, body);
            }
        }

        return (true, "Deposit confirmed successfully");
    }

    public async Task<BookingDetailVO?> GetBookingInformationByCarId(Guid carId)
    {
        var bookingEntities = await _bookingRepository.GetBookingsByCarIdAsync(carId);

        var booking = bookingEntities
            .FirstOrDefault(b => b.Status == BookingStatusEnum.pending_deposit.ToString());

        return booking != null ? BookingMapper.ToBookingDetailVO(booking) : null;
    }



}
