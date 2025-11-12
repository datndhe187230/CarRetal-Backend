using CarRental_BE.Exceptions;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Enum;
using CarRental_BE.Models.VO.Booking;
using CarRental_BE.Repositories;
using CarRental_BE.Services;
using CarRental_BE.Models.NewEntities;
using CarRental_BE.Models.Helpers;

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

    public async Task<List<BookingVO>> GetBookingsByAccountIdAsync(Guid accountId, BookingQueryDto queryDto)
    {
        var bookingEntities = await _bookingRepository.GetBookingsByAccountIdAsync(accountId);

        // Chuyển sang List và áp dụng filter
        var filteredBookings = ApplyFilters(bookingEntities.AsQueryable(), queryDto).ToList();
        var sortedBookings = ApplySorting(filteredBookings.AsQueryable(), queryDto.SortOrder).ToList();

        return sortedBookings.Select(BookingMapper.ToBookingVO).ToList();
    }

    private IQueryable<Booking> ApplyFilters(IQueryable<Booking> bookings, BookingQueryDto queryDto)
    {
        var filtered = bookings;

        // Filter by search term
        if (!string.IsNullOrEmpty(queryDto.SearchTerm))
        {
            var searchTerm = queryDto.SearchTerm.ToLower();

            filtered = filtered.Where(b =>
                (b.Car.Brand ?? "").ToLower().Contains(searchTerm) ||
                (b.Car.Model ?? "").ToLower().Contains(searchTerm) ||
                ((b.Car.Brand + " " + b.Car.Model) ?? "").ToLower().Contains(searchTerm) || // Brand + Model
                (b.BookingNumber ?? "").ToLower().Contains(searchTerm));


        }

        // Filter by status
        if (queryDto.Statuses != null && queryDto.Statuses.Any())
        {
            filtered = filtered.Where(b => queryDto.Statuses.Contains(b.Status ?? ""));
        }

        return filtered;
    }

    private IQueryable<Booking> ApplySorting(IQueryable<Booking> bookings, string? sortOrder)
    {
        return sortOrder?.ToLower() switch
        {
            "oldest" => bookings.OrderBy(b => b.CreatedAt),
            "newest" => bookings.OrderByDescending(b => b.CreatedAt),
            _ => bookings.OrderByDescending(b => b.CreatedAt) // default: newest first
        };
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
            var owner = await _accountRepository.GetByIdAsync(car.OwnerAccountId);
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

        // Calculate final total using calculator (ensures extra km and discounts are applied)
        var totalAmount = BookingPriceCalculator.CalculateTotalCents(booking);
        var deposit = booking.DepositSnapshotCents ??0m;

        var customer = await _accountRepository.GetByIdAsync(booking.RenterAccountId);
        if (customer?.Wallet == null)
            return (false, "Customer wallet not found");

        //1. Create transaction for remaining payment or refund
        if (totalAmount > deposit)
        {
            var diff = totalAmount - deposit;
            if (customer.Wallet.BalanceCents < diff)
            {
                booking.Status = "pending_payment";
                await _bookingRepository.UpdateAsync(booking);
                return (false, "ME012: Your wallet doesn’t have enough balance. Please top-up your wallet and try again.");
            }

            // Deduct remaining amount from wallet and create transaction
            await _walletService.WithdrawMoney(customer.AccountId, new TransactionDTO
            {
                Amount = (long)diff,
                Message = "Pay remaining rental fee",
                BookingId = booking.BookingNumber,
                CarName = booking.Car != null ? $"{booking.Car.Brand} {booking.Car.Model} {booking.Car.ProductionYear} {booking.Car.LicensePlate}" : "",
                Type = TransactionType.offset_final_payment,
                Status = TransactionStatus.Successful
            });
        }
        else if (deposit > totalAmount)
        {
            var refund = deposit - totalAmount;
            await _walletService.TopupMoney(customer.AccountId, new TransactionDTO
            {
                Amount = (long)refund,
                Message = "Refund excess deposit",
                BookingId = booking.BookingNumber,
                CarName = booking.Car != null ? $"{booking.Car.Brand} {booking.Car.Model} {booking.Car.ProductionYear} {booking.Car.LicensePlate}" : "",
                Type = TransactionType.offset_final_payment,
                Status = TransactionStatus.Successful
            });
        }

        //2. Update all pending deposit transactions to successful
        var depositTransactions = booking.Transactions
            .Where(t =>
                (t.Type == TransactionType.pay_deposit.ToString() ||
                 t.Type == TransactionType.receive_deposit.ToString()) &&
                t.Status == TransactionStatus.Processing.ToString())
            .ToList();

        foreach (var transaction in depositTransactions)
        {
            await _walletService.UpdateTransactionStatusAsync(transaction.TransactionId, TransactionStatus.Successful.ToString());
        }

        booking.Status = BookingStatusEnum.completed.ToString();
        await _bookingRepository.UpdateAsync(booking);

        var car = booking.Car ?? await _carRepository.GetByIdAsync(booking.CarId);
        var owner = car != null ? await _accountRepository.GetByIdAsync(car.OwnerAccountId) : null;

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

                    // Determine active pricing plan
                    var plan = car.CarPricingPlans?.FirstOrDefault(p => p.IsActive == true);
                    if (plan == null)
                    {
                        throw new System.InvalidOperationException("No active pricing plan for this car.");
                    }
                    var days = BookingPriceCalculator.CalculateTotalDays(bookingCreateDto.PickupDate, bookingCreateDto.DropoffDate);
                    decimal basePrice = plan.BasePricePerDayCents * days;

                    var toCreate = BookingMapper.ToBookingEntity(bookingCreateDto, userId, BookingStatusEnum.pending_deposit, basePrice, bookingNumber);
                    toCreate.PricingPlanId = plan.PlanId;
                    toCreate.DepositSnapshotCents = plan.DepositCents;

                    var created = await _bookingRepository.CreateBookingAsync(toCreate);
                    if (created == null)
                    {
                        throw new System.InvalidOperationException("Failed to create booking.");
                    }

                    // Commit the transaction for cash payment
                    await transaction.CommitAsync();

                    return BookingMapper.ToBookingVO(created);
                }
                else if (bookingCreateDto.PaymentType == "wallet")
                {
                    await CheckWallet(userId, bookingCreateDto);

                    string bookingNumber = await GenerateBookingNumberAsync();

                    // Determine active pricing plan
                    var plan = car.CarPricingPlans?.FirstOrDefault(p => p.IsActive == true);
                    if (plan == null)
                    {
                        throw new System.InvalidOperationException("No active pricing plan for this car.");
                    }
                    var days = BookingPriceCalculator.CalculateTotalDays(bookingCreateDto.PickupDate, bookingCreateDto.DropoffDate);
                    decimal basePrice = plan.BasePricePerDayCents * days;

                    if (((double)bookingCreateDto.Deposit) < ((double)basePrice *0.3))
                    {
                        throw new System.InvalidOperationException("Deposit amount must be greater than30% of total");
                    }

                    var toCreate = BookingMapper.ToBookingEntity(bookingCreateDto, userId, BookingStatusEnum.confirmed, basePrice, bookingNumber);
                    toCreate.PricingPlanId = plan.PlanId;
                    toCreate.DepositSnapshotCents = plan.DepositCents;

                    var created = await _bookingRepository.CreateBookingAsync(toCreate);
                    if (created == null)
                    {
                        throw new System.InvalidOperationException("Failed to create booking.");
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

                    // Add90% deposit to car owner balance
                    TransactionDTO topupDTO = new TransactionDTO
                    {
                        Amount = (long)(bookingCreateDto.Deposit *0.9m),
                        Message = "Booking deposit paid",
                        BookingId = bookingNumber,
                        CarName = $"{car.Brand} {car.Model} {car.ProductionYear} {car.LicensePlate}",
                        Type = TransactionType.receive_deposit,
                        Status = TransactionStatus.Processing
                    };
                    await _walletService.TopupMoney(car.OwnerAccountId, topupDTO);

                    // Add10% deposit to admin balance
                    TransactionDTO topupAdminDTO = new TransactionDTO
                    {
                        Amount = (long)(bookingCreateDto.Deposit *0.1m),
                        Message = "Booking deposit commission",
                        BookingId = bookingNumber,
                        CarName = $"{car.Brand} {car.Model} {car.ProductionYear} {car.LicensePlate}",
                        Type = TransactionType.receive_deposit
                    };
                    await _walletService.TopupMoneyAdmin(topupAdminDTO);

                    // Commit the transaction for wallet payment
                    await transaction.CommitAsync();

                    return BookingMapper.ToBookingVO(created);
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
        if (bookingCreateDto.Deposit <0)
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
            var owner = await _accountRepository.GetByIdAsync(car.OwnerAccountId);
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
