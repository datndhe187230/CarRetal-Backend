using CarRental_BE.Exceptions;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Enum;
using CarRental_BE.Models.VO.Booking;
using CarRental_BE.Repositories;
using CarRental_BE.Services;
using CarRental_BE.Models.NewEntities;
using CarRental_BE.Models.Helpers;
using CarRental_BE.Helpers;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using CarRental_BE.Services.Background.Email;

/// 
/// Booking service: contains booking querying, creation, and the full status flow with
/// transactional updates, history logging, wallet settlement hooks, and email notifications.
/// 
public class BookingServiceImpl : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICarRepository _carRepository;
    private readonly IEmailService _emailService;
    private readonly IWalletService _walletService;
    private readonly ILogger<BookingServiceImpl> _logger;
    private readonly IEmailJobQueue _emailJobQueue;

    public BookingServiceImpl(
        IBookingRepository bookingRepository,
        IAccountRepository accountRepository,
        ICarRepository carRepository,
        IEmailService emailService,
        IWalletService walletService,
        ILogger<BookingServiceImpl> logger,
        IEmailJobQueue emailJobQueue)
    {
        _bookingRepository = bookingRepository;
        _accountRepository = accountRepository;
        _carRepository = carRepository;
        _emailService = emailService;
        _walletService = walletService;
        _logger = logger;
        _emailJobQueue = emailJobQueue;
    }

    // -------------------- Read APIs --------------------

    /// Get all bookings (admin/debug).
    public async Task<List<BookingVO>> GetAllBookingsAsync()
    {
        var bookingEntities = await _bookingRepository.GetAllBookingsAsync();
        return bookingEntities.Select(BookingMapper.ToBookingVO).ToList();
    }

    /// Get bookings by renter account.
    public async Task<List<BookingVO>> GetBookingsByAccountIdAsync(Guid accountId)
    {
        var bookingEntities = await _bookingRepository.GetBookingsByAccountIdAsync(accountId);
        return bookingEntities.Select(BookingMapper.ToBookingVO).ToList();
    }

    /// Get bookings by renter account with filters and sorting.
    public async Task<List<BookingVO>> GetBookingsByAccountIdAsync(Guid accountId, BookingQueryDto queryDto)
    {
        var bookingEntities = await _bookingRepository.GetBookingsByAccountIdAsync(accountId);
        var filtered = ApplyFilters(bookingEntities.AsQueryable(), queryDto);
        var sorted = ApplySorting(filtered, queryDto.SortOrder);
        return sorted.Select(BookingMapper.ToBookingVO).ToList();
    }

    /// Get bookings with pagination (admin view).
    public async Task<(List<BookingVO>, int)> GetBookingsWithPagingAsync(int page, int pageSize)
    {
        var (entities, totalCount) = await _bookingRepository.GetBookingsWithPagingAsync(page, pageSize);
        return (entities.Select(BookingMapper.ToBookingVO).ToList(), totalCount);
    }

    /// Get booking details by booking number (includes history timeline, driver, car, etc.).
    public async Task<BookingDetailVO?> GetBookingByBookingNumberAsync(string id)
    {
        var entity = await _bookingRepository.GetBookingByBookingNumberAsync(id);
        return entity != null ? BookingMapper.ToBookingDetailVO(entity) : null;
    }

    /// Update editable fields of a booking (driver info etc.).
    public async Task<BookingDetailVO?> UpdateBookingAsync(string bookingNumber, BookingEditDTO bookingDto)
    {
        var updated = await _bookingRepository.UpdateBookingAsync(bookingNumber, bookingDto);
        return updated != null ? BookingMapper.ToBookingDetailVO(updated) : null;
    }

    /// Get occupied date ranges for a car (calendar feed).
    public async Task<OccupiedDateRange[]> GetOccupiedDatesByCarId(Guid carId)
    {
        var bookings = await _bookingRepository.GetBookingsByCarId(carId);
        return bookings.Select(b => new OccupiedDateRange
        {
            Start = b.PickUpTime,
            End = b.ActualReturnTime ?? b.DropOffTime
        }).ToArray();
    }

    /// Get the booking (if any) currently waiting for cash deposit for a specific car.
    public async Task<BookingDetailVO?> GetBookingInformationByCarId(Guid carId)
    {
        var bookingEntities = await _bookingRepository.GetBookingsByCarIdAsync(carId);
        var booking = bookingEntities.FirstOrDefault(b => b.Status == BookingStatusFlow.PendingDeposited);
        return booking != null ? BookingMapper.ToBookingDetailVO(booking) : null;
    }

    // -------------------- Creation --------------------

    /// Create a booking for the given user and request.
    /// - cash: starts in waiting_confirmed
    /// - wallet: starts in confirmed and creates deposit transactions (Processing)
    public async Task<BookingVO?> CreateBookingAsync(Guid userId, BookingCreateDTO dto)
    {
        using var tx = await _bookingRepository.BeginTransactionAsync();
        try
        {
            var car = await _carRepository.GetByIdAsync(dto.CarId) ?? throw new ArgumentException("Car not found.");
            var plan = car.CarPricingPlans?.FirstOrDefault(p => p.IsActive == true) ?? throw new System.InvalidOperationException("No active pricing plan for this car.");
            var days = BookingPriceCalculator.CalculateTotalDays(dto.PickupDate, dto.DropoffDate);
            var basePrice = plan.BasePricePerDayCents * days;
            var bookingNumber = await GenerateBookingNumberAsync();

            // Create pickup and dropoff addresses
            var pickUpAddr = await _bookingRepository.CreateAddressAsync(new Address
            {
                AddressId = Guid.Empty,
                HouseNumberStreet = string.Empty,
                Ward = dto.PickupLocation?.Ward ?? string.Empty,
                District = dto.PickupLocation?.District ?? string.Empty,
                CityProvince = dto.PickupLocation?.Province ?? string.Empty
            });
            var dropOffAddr = await _bookingRepository.CreateAddressAsync(new Address
            {
                AddressId = Guid.Empty,
                HouseNumberStreet = string.Empty,
                Ward = dto.DropoffLocation?.Ward ?? string.Empty,
                District = dto.DropoffLocation?.District ?? string.Empty,
                CityProvince = dto.DropoffLocation?.Province ?? string.Empty
            });

            // Map entity and attach addresses
            var entity = BookingMapper.ToBookingEntity(dto, userId, BookingStatusEnum.waiting_confirmed, basePrice, bookingNumber);
            entity.PricingPlanId = plan.PlanId;
            entity.DepositSnapshotCents = plan.DepositCents;
            entity.PickUpAddressId = pickUpAddr.AddressId;
            entity.DropOffAddressId = dropOffAddr.AddressId;

            // Determine if driver is different from renter based on the flag provided by client
            var driverDifferent = dto.IsDifferentDriver;

            if (dto.PaymentType == "cash")
            {
                // Cash path: wait for owner confirm to move to pending_deposited
                var created = await _bookingRepository.CreateBookingAsync(entity) ?? throw new System.InvalidOperationException("Failed to create booking.");

                // Create driver only when different from renter
                if (driverDifferent)
                {
                    var driverAddress = await _bookingRepository.CreateAddressAsync(new Address
                    {
                        AddressId = Guid.Empty,
                        HouseNumberStreet = dto.DriverHouseNumberStreet ?? string.Empty,
                        Ward = dto.DriverLocation?.Ward ?? string.Empty,
                        District = dto.DriverLocation?.District ?? string.Empty,
                        CityProvince = dto.DriverLocation?.Province ?? string.Empty
                    });
                    await _bookingRepository.CreateBookingDriverAsync(new BookingDriver
                    {
                        BookingNumber = bookingNumber,
                        AccountId = null,
                        FullName = dto.DriverFullName,
                        Dob = dto.DriverDob.HasValue ? DateOnly.FromDateTime(dto.DriverDob.Value) : null,
                        PhoneNumber = dto.DriverPhoneNumber,
                        NationalId = dto.DriverNationalId,
                        DrivingLicenseUri = dto.DriverDrivingLicenseUri,
                        AddressId = driverAddress.AddressId
                    });
                }

                await tx.CommitAsync();
                return BookingMapper.ToBookingVO(created);
            }
            else if (dto.PaymentType == "wallet")
            {
                // Wallet path: ensures deposit availability and creates Processing transactions
                await CheckWallet(userId, dto);
                if ((int)dto.Deposit <= (int)basePrice * 0.3) throw new System.InvalidOperationException("Deposit must be at least30% of total base price.");

                var created = await _bookingRepository.CreateBookingAsync(entity) ?? throw new System.InvalidOperationException("Failed to create booking.");

                // Create driver only when different from renter
                if (driverDifferent)
                {
                    var driverAddress = await _bookingRepository.CreateAddressAsync(new Address
                    {
                        AddressId = Guid.Empty,
                        HouseNumberStreet = dto.DriverHouseNumberStreet ?? string.Empty,
                        Ward = dto.DriverLocation?.Ward ?? string.Empty,
                        District = dto.DriverLocation?.District ?? string.Empty,
                        CityProvince = dto.DriverLocation?.Province ?? string.Empty
                    });
                    await _bookingRepository.CreateBookingDriverAsync(new BookingDriver
                    {
                        BookingNumber = bookingNumber,
                        AccountId = null,
                        FullName = dto.DriverFullName,
                        Dob = dto.DriverDob.HasValue ? DateOnly.FromDateTime(dto.DriverDob.Value) : null,
                        PhoneNumber = dto.DriverPhoneNumber,
                        NationalId = dto.DriverNationalId,
                        DrivingLicenseUri = dto.DriverDrivingLicenseUri,
                        AddressId = driverAddress.AddressId
                    });
                }

                // Record wallet transactions (Processing). Finalization happens at completion/cancellation.
                await _walletService.WithdrawMoney(userId, new TransactionDTO
                {
                    Amount = (long)dto.Deposit,
                    Message = "Booking deposit",
                    BookingId = bookingNumber,
                    CarName = $"{car.Brand} {car.Model} {car.ProductionYear} {car.LicensePlate}",
                    Type = TransactionType.pay_deposit,
                    Status = TransactionStatus.Processing
                });
                await _walletService.TopupMoney(car.OwnerAccountId, new TransactionDTO
                {
                    Amount = (long)(dto.Deposit * 0.9m),
                    Message = "Booking deposit paid",
                    BookingId = bookingNumber,
                    CarName = $"{car.Brand} {car.Model} {car.ProductionYear} {car.LicensePlate}",
                    Type = TransactionType.receive_deposit,
                    Status = TransactionStatus.Processing
                });
                await _walletService.TopupMoneyAdmin(new TransactionDTO
                {
                    Amount = (long)(dto.Deposit * 0.1m),
                    Message = "Booking deposit commission",
                    BookingId = bookingNumber,
                    CarName = $"{car.Brand} {car.Model} {car.ProductionYear} {car.LicensePlate}",
                    Type = TransactionType.receive_deposit,
                    Status = TransactionStatus.Processing
                });

                await tx.CommitAsync();
                return BookingMapper.ToBookingVO(created);
            }
            else
            {
                throw new ArgumentException("Invalid payment type. Must be 'cash' or 'wallet'.");
            }
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    // -------------------- Status Flow --------------------

    /// Owner confirms booking:
    /// - cash -> waiting_confirm -> pending_deposited
    /// - wallet -> waiting_confirm -> confirmed
    public async Task<(bool Success, string Message)> ConfirmBookingAsync(string bookingNumber)
    {
        using var tx = await _bookingRepository.BeginTransactionAsync();
        try
        {
            var booking = await _bookingRepository.GetByBookingNumberAsync(bookingNumber);
            if (booking == null) return (false, "Booking not found");
            if (!string.Equals(booking.Status, BookingStatusFlow.WaitingConfirm, StringComparison.OrdinalIgnoreCase))
                return (false, "Only 'waiting_confirm' can be confirmed");

            if (string.Equals(booking.PaymentMethod, "cash", StringComparison.OrdinalIgnoreCase))
            {
                await TransitionAsync(booking, BookingStatusFlow.PendingDeposited, "Owner confirmed booking; awaiting cash deposit");
            }
            else if (string.Equals(booking.PaymentMethod, "wallet", StringComparison.OrdinalIgnoreCase))
            {
                await TransitionAsync(booking, BookingStatusFlow.Confirmed, "Wallet deposit authorized");
            }
            else return (false, "Unsupported payment method");

            await tx.CommitAsync();
            return (true, "Confirmed");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return (false, ex.Message);
        }
    }

    /// Owner confirms that the cash deposit has been received: pending_deposited -> confirmed.
    public async Task<(bool Success, string Message)> ConfirmDepositAsync(string bookingNumber)
    {
        using var tx = await _bookingRepository.BeginTransactionAsync();
        try
        {
            var booking = await _bookingRepository.GetByBookingNumberAsync(bookingNumber);
            if (booking == null) return (false, "Booking not found");
            if (!string.Equals(booking.Status, BookingStatusFlow.PendingDeposited, StringComparison.OrdinalIgnoreCase))
                return (false, "Only 'pending_deposited' bookings can be confirmed");

            await TransitionAsync(booking, BookingStatusFlow.Confirmed, "Owner confirmed cash deposit");
            await tx.CommitAsync();
            return (true, "Deposit confirmed");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return (false, ex.Message);
        }
    }

    /// Customer confirms pickup: confirmed -> in_progress (guard: now >= PickUpTime).
    public async Task<(bool Success, string Message)> ConfirmPickupAsync(string bookingNumber)
    {
        using var tx = await _bookingRepository.BeginTransactionAsync();
        try
        {
            var booking = await _bookingRepository.GetByBookingNumberAsync(bookingNumber);
            if (booking == null) return (false, "Booking not found");
            if (!string.Equals(booking.Status, BookingStatusFlow.Confirmed, StringComparison.OrdinalIgnoreCase))
                return (false, "Only 'confirmed' bookings can be picked up");
            if (DateTime.UtcNow < booking.PickUpTime)
                return (false, "Pickup cannot be confirmed before scheduled time");

            await TransitionAsync(booking, BookingStatusFlow.InProgress, "Customer confirmed pickup");
            await tx.CommitAsync();
            return (true, "In progress");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return (false, ex.Message);
        }
    }

    /// Customer requests return: in_progress -> waiting_confirm_return.
    public async Task<(bool Success, string Message)> RequestReturnAsync(string bookingNumber)
    {
        using var tx = await _bookingRepository.BeginTransactionAsync();
        try
        {
            var booking = await _bookingRepository.GetByBookingNumberAsync(bookingNumber);
            if (booking == null) return (false, "Booking not found");
            if (!string.Equals(booking.Status, BookingStatusFlow.InProgress, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(booking.Status, BookingStatusFlow.RejectedReturn, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(booking.Status, BookingStatusFlow.PendingPayment, StringComparison.OrdinalIgnoreCase))
                return (false, "Only 'in_progress', 'rejected' or 'pending_payment' can request return");

            // Compute remaining amount to settle (after deposit)
            booking.ActualReturnTime = DateTime.UtcNow;
            var car = booking.Car ?? await _carRepository.GetByIdAsync(booking.CarId);
            var total = BookingPriceCalculator.CalculateTotalCents(booking);
            var deposit = booking.DepositSnapshotCents ?? 0m;
            var remainingToPay = Math.Max(0m, total - deposit);

            if (remainingToPay > 0)
            {
                var renterBalance = (await _walletService.GetWalletBalance(booking.RenterAccountId)).Balance;
                if (renterBalance < remainingToPay)
                {
                    // Insufficient funds -> move to pending_payment
                    if (BookingStatusFlow.IsValidTransition(booking.Status, BookingStatusFlow.PendingPayment))
                    {
                        await TransitionAsync(booking, BookingStatusFlow.PendingPayment, "Insufficient wallet balance. Pending payment.");
                        await tx.CommitAsync();
                        return (true, "Pending payment");
                    }
                    return (false, "Insufficient wallet balance. Unable to request return.");
                }

                // Charge remaining amount as Processing and split90/10 to owner/admin
                await _walletService.WithdrawMoney(booking.RenterAccountId, new TransactionDTO
                {
                    Amount = (long)remainingToPay,
                    Message = "Remaining rental fee",
                    BookingId = booking.BookingNumber,
                    CarName = car != null ? $"{car.Brand} {car.Model} {car.ProductionYear} {car.LicensePlate}" : "",
                    Type = TransactionType.offset_final_payment,
                    Status = TransactionStatus.Processing
                });
                await _walletService.TopupMoney(car!.OwnerAccountId, new TransactionDTO
                {
                    Amount = (long)(remainingToPay * 0.9m),
                    Message = "Owner receives final payment (90%)",
                    BookingId = booking.BookingNumber,
                    CarName = car != null ? $"{car.Brand} {car.Model} {car.ProductionYear} {car.LicensePlate}" : "",
                    Type = TransactionType.offset_final_payment,
                    Status = TransactionStatus.Processing
                });
                await _walletService.TopupMoneyAdmin(new TransactionDTO
                {
                    Amount = (long)(remainingToPay * 0.1m),
                    Message = "Admin commission from final payment (10%)",
                    BookingId = booking.BookingNumber,
                    CarName = car != null ? $"{car.Brand} {car.Model} {car.ProductionYear} {car.LicensePlate}" : "",
                    Type = TransactionType.offset_final_payment,
                    Status = TransactionStatus.Processing
                });
            }

            await TransitionAsync(booking, BookingStatusFlow.WaitingConfirmReturn, "Customer requested return");
            await tx.CommitAsync();
            return (true, "Waiting confirm return");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return (false, ex.Message);
        }
    }

    /// Owner accepts return: waiting_confirm_return -> completed (sets ActualReturnTime, settlement TODO).
    public async Task<(bool Success, string Message)> AcceptReturnAsync(string bookingNumber, string? note = null, string? pictureUrl = null, decimal? chargesCents = null)
    {
        using var tx = await _bookingRepository.BeginTransactionAsync();
        try
        {
            var booking = await _bookingRepository.GetByBookingNumberAsync(bookingNumber);
            if (booking == null) return (false, "Booking not found");
            if (!string.Equals(booking.Status, BookingStatusFlow.WaitingConfirmReturn, StringComparison.OrdinalIgnoreCase))
                return (false, "Only 'waiting_confirm_return' can be completed");

            // Money settlement finalization
            booking.ActualReturnTime = DateTime.UtcNow;
            var total = BookingPriceCalculator.CalculateTotalCents(booking);
            var deposit = booking.DepositSnapshotCents ?? 0m;
            var remainingToPay = Math.Max(0m, total - deposit);
            var refund = Math.Max(0m, deposit - total);

            // Refund any excess deposit to renter
            if (refund > 0)
            {
                await _walletService.TopupMoney(booking.RenterAccountId, new TransactionDTO
                {
                    Amount = (long)refund,
                    Message = "Refund excess deposit",
                    BookingId = booking.BookingNumber,
                    CarName = booking.Car != null ? $"{booking.Car.Brand} {booking.Car.Model} {booking.Car.ProductionYear} {booking.Car.LicensePlate}" : "",
                    Type = TransactionType.offset_final_payment,
                    Status = TransactionStatus.Successful
                });
            }

            // Mark all Processing transactions for this booking as Successful (deposit + remaining payment)
            if (booking.Transactions != null)
            {
                foreach (var t in booking.Transactions.Where(t => t.Status == TransactionStatus.Processing.ToString()))
                {
                    await _walletService.UpdateTransactionStatusAsync(t.TransactionId, TransactionStatus.Successful.ToString());
                }
            }

            // Compose revenue summary
            var ownerShareRemaining = (long)(remainingToPay * 0.9m);
            var adminShareRemaining = (long)(remainingToPay * 0.1m);
            var summary = $"Settlement summary: Total={total:N0}¢, Deposit={deposit:N0}¢, Remaining Charged={remainingToPay:N0}¢ (Owner90%={ownerShareRemaining:N0}¢, Admin10%={adminShareRemaining:N0}¢), Refund={refund:N0}¢";

            await TransitionAsync(booking, BookingStatusFlow.Completed, note ?? summary, pictureUrl);
            await tx.CommitAsync();
            return (true, "Completed");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return (false, ex.Message);
        }
    }

    /// Owner rejects return: waiting_confirm_return -> rejected_return.
    public async Task<(bool Success, string Message)> RejectReturnAsync(string bookingNumber, string? note = null, string? pictureUrl = null)
    {
        using var tx = await _bookingRepository.BeginTransactionAsync();
        try
        {
            var booking = await _bookingRepository.GetByBookingNumberAsync(bookingNumber);
            if (booking == null) return (false, "Booking not found");
            if (!string.Equals(booking.Status, BookingStatusFlow.WaitingConfirmReturn, StringComparison.OrdinalIgnoreCase))
                return (false, "Only 'waiting_confirm_return' can be rejected");

            // Revert only the remaining-payment transactions created at return time
            await _walletService.RevertBookingTransactionsByTypeAsync(booking.BookingNumber, TransactionType.offset_final_payment);

            await TransitionAsync(booking, BookingStatusFlow.RejectedReturn, note ?? "Owner rejected return", pictureUrl);
            await tx.CommitAsync();
            return (true, "Rejected return");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return (false, ex.Message);
        }
    }


    /// Customer cancels booking. If confirmed and wallet, customer forfeits deposit.
    /// Otherwise, no financial operations are needed.
    public async Task<(bool Success, string Message)> CustomerCancelAsync(string bookingNumber, string? reason = null, string? pictureUrl = null)
        => await CancelInternalAsync(bookingNumber, reason, pictureUrl, initiatedByOwner: false);

    /// Owner cancels booking. If confirmed and wallet, renter receives full deposit refund.

    public async Task<(bool Success, string Message)> OwnerCancelAsync(string bookingNumber, string? reason = null, string? pictureUrl = null)
        => await CancelInternalAsync(bookingNumber, reason, pictureUrl, initiatedByOwner: true);


    // -------------------- Helpers --------------------

    private IQueryable<Booking> ApplyFilters(IQueryable<Booking> bookings, BookingQueryDto queryDto)
    {
        if (!string.IsNullOrEmpty(queryDto.SearchTerm))
        {
            var term = queryDto.SearchTerm.ToLower();
            bookings = bookings.Where(b =>
                (b.Car.Brand ?? "").ToLower().Contains(term) ||
                (b.Car.Model ?? "").ToLower().Contains(term) ||
                ((b.Car.Brand + " " + b.Car.Model) ?? "").ToLower().Contains(term) ||
                (b.BookingNumber ?? "").ToLower().Contains(term));
        }
        if (queryDto.Statuses != null && queryDto.Statuses.Any())
            bookings = bookings.Where(b => queryDto.Statuses.Contains(b.Status ?? ""));
        return bookings;
    }

    private IQueryable<Booking> ApplySorting(IQueryable<Booking> bookings, string? sortOrder)
    {
        return sortOrder?.ToLower() switch
        {
            "oldest" => bookings.OrderBy(b => b.CreatedAt),
            "newest" => bookings.OrderByDescending(b => b.CreatedAt),
            _ => bookings.OrderByDescending(b => b.CreatedAt)
        };
    }

    private async Task<string> GenerateBookingNumberAsync()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var seq = await _bookingRepository.GetNextBookingSequenceForDateAsync(datePart);
        return $"{datePart}-{seq}";
    }

    /// Checks customer wallet balance for the provided deposit amount.
    private async Task CheckWallet(Guid userId, BookingCreateDTO dto)
    {
        var balance = (await _walletService.GetWalletBalance(userId)).Balance;
        if (dto.Deposit < 0) throw new ArgumentException("Deposit cannot be negative.");
        if (balance < dto.Deposit) throw new ArgumentException("Insufficient wallet balance.");
    }

    /// Cancellation handler with refund policy:
    /// - waiting_confirm/pending_deposited: no wallet operations.
    /// - confirmed + customer: deposit forfeited (mark deposit transactions successful).
    /// - confirmed + owner: refund full deposit to renter (revert transactions + renter topup).

    private async Task<(bool Success, string Message)> CancelInternalAsync(string bookingNumber, string? reason, string? pictureUrl, bool initiatedByOwner)
    {
        using var tx = await _bookingRepository.BeginTransactionAsync();
        try
        {
            var booking = await _bookingRepository.GetByBookingNumberAsync(bookingNumber);
            if (booking == null) return (false, "Booking not found");

            var status = booking.Status;
            var oldStatus = status;
            bool isConfirmed = string.Equals(status, BookingStatusFlow.Confirmed, StringComparison.OrdinalIgnoreCase);
            bool allowedPreConfirmed = string.Equals(status, BookingStatusFlow.WaitingConfirm, StringComparison.OrdinalIgnoreCase)
                || string.Equals(status, BookingStatusFlow.PendingDeposited, StringComparison.OrdinalIgnoreCase);

            if (!allowedPreConfirmed && !isConfirmed)
                return (false, "Cancellation only allowed in 'waiting_confirm', 'pending_deposited', or 'confirmed'.");

            // Wallet deposit policy only applies when already confirmed
            if (isConfirmed && string.Equals(booking.PaymentMethod, "wallet", StringComparison.OrdinalIgnoreCase))
            {
                var depositAmount = booking.DepositSnapshotCents ?? 0m;

                if (initiatedByOwner)
                {
                    // Owner cancels after confirmed: refund full deposit to renter
                    if (depositAmount > 0)
                    {
                        await _walletService.RevertBookingTransactionsAsync(booking.BookingNumber);
                        await _walletService.TopupMoney(booking.RenterAccountId, new TransactionDTO
                        {
                            Amount = (long)depositAmount,
                            Message = "Refund deposit due to owner cancellation",
                            BookingId = booking.BookingNumber,
                            CarName = booking.Car != null ? $"{booking.Car.Brand} {booking.Car.Model} {booking.Car.ProductionYear} {booking.Car.LicensePlate}" : "",
                            Type = TransactionType.offset_final_payment,
                            Status = TransactionStatus.Successful
                        });
                    }
                }
                else
                {
                    // Customer cancels after confirmed: deposit forfeited
                    foreach (var t in booking.Transactions.Where(t =>
                        (t.Type == TransactionType.pay_deposit.ToString() || t.Type == TransactionType.receive_deposit.ToString())
                        && t.Status == TransactionStatus.Processing.ToString()))
                    {
                        await _walletService.UpdateTransactionStatusAsync(t.TransactionId, TransactionStatus.Successful.ToString());
                    }
                }
            }

            // Status change + history + emails
            await TransitionAsync(booking, BookingStatusFlow.Cancelled, reason, pictureUrl);
            // No await: send emails in background as part of TransitionAsync

            await tx.CommitAsync();
            return (true, "Cancelled");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return (false, ex.Message);
        }
    }

    /// Transition helper: validates allowed transition, updates booking status,
    /// appends history, and sends notification emails.

    private async Task TransitionAsync(Booking booking, string newStatus, string? note = null, string? pictureUrl = null)
    {
        if (!BookingStatusFlow.IsValidTransition(booking.Status, newStatus))
            throw new System.InvalidOperationException($"Transition from '{booking.Status}' to '{newStatus}' is not allowed");

        var oldStatus = booking.Status;
        var history = new BookingStatusHistory
        {
            BookingNumber = booking.BookingNumber,
            OldStatus = booking.Status,
            NewStatus = newStatus,
            Note = note,
            PictureUrl = pictureUrl,
            ChangedAt = DateTime.UtcNow
        };

        var ok = await _bookingRepository.UpdateBookingStatusAsync(booking.BookingNumber, newStatus);
        if (!ok) throw new System.InvalidOperationException("Failed to update booking status");

        await _bookingRepository.AddStatusHistoryAsync(history);

        // Fire-and-forget via background queue (do not await sending)
        await _emailJobQueue.EnqueueAsync(new StatusChangeEmailJob
        {
            BookingNumber = booking.BookingNumber,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            Note = note
        });
    }

    public async Task<BookingSummaryVO?> GetBookingSummaryAsync(string bookingNumber)
    {
        var booking = await _bookingRepository.GetByBookingNumberAsync(bookingNumber);
        if (booking == null) return null;

        var plan = booking.PricingPlan;
        if (plan == null)
        {
            // in case not loaded, try fetching
            var car = await _carRepository.GetByIdAsync(booking.CarId);
            plan = car?.CarPricingPlans?.FirstOrDefault(p => p.PlanId == booking.PricingPlanId);
        }
        if (plan == null) return null;

        var days = BookingPriceCalculator.CalculateTotalDays(booking.PickUpTime, booking.DropOffTime);
        var basePerDay = plan.BasePricePerDayCents;
        var basePrice = basePerDay * days;
        var deposit = booking.DepositSnapshotCents ?? plan.DepositCents;

        // recompute parts similar to calculator
        decimal extraKmFee = 0m;
        if (booking.KmDriven.HasValue && plan.KmIncludedDaily.HasValue && plan.PricePerExtraKmCents.HasValue)
        {
            var allowedKm = plan.KmIncludedDaily.Value * days;
            var extraKm = Math.Max(0m, booking.KmDriven.Value - allowedKm);
            extraKmFee = extraKm * (plan.PricePerExtraKmCents.Value);
        }
        decimal discount = 0m;
        if (plan.DiscountPercent.HasValue && plan.DiscountPercent.Value > 0)
        {
            discount = basePrice * (plan.DiscountPercent.Value / 100m);
        }
        var extraCharges = booking.ExtraChargesCents ?? 0m;
        var total = basePrice + extraKmFee + deposit - discount + extraCharges;

        var remaining = Math.Max(0m, total - deposit);
        var refund = Math.Max(0m, deposit - total);

        var summary = new BookingSummaryVO
        {
            BookingNumber = booking.BookingNumber,
            Status = booking.Status,
            PickUpTime = booking.PickUpTime,
            DropOffTime = booking.DropOffTime,
            ActualReturnTime = booking.ActualReturnTime,
            BasePricePerDayCents = basePerDay,
            TotalDays = days,
            BasePriceCents = basePrice,
            DepositSnapshotCents = deposit,
            ExtraKmFeeCents = extraKmFee,
            DiscountCents = discount,
            ExtraChargesCents = extraCharges,
            TotalCalculatedCents = total,
            RemainingChargedCents = remaining,
            RefundToRenterCents = refund,
            OwnerShareFromDepositCents = deposit * 0.9m,
            AdminShareFromDepositCents = deposit * 0.1m,
            OwnerShareFromRemainingCents = remaining * 0.9m,
            AdminShareFromRemainingCents = remaining * 0.1m,
            Timeline = booking.BookingStatusHistories?.OrderBy(h => h.ChangedAt).Select(h => new BookingStatusHistoryVO
            {
                OldStatus = h.OldStatus,
                NewStatus = h.NewStatus,
                Note = h.Note,
                PictureUrl = h.PictureUrl,
                ChangedAt = h.ChangedAt
            }).ToList()
        };
        return summary;
    }
}
