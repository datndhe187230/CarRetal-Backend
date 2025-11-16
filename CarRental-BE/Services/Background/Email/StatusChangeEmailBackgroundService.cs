using System.Net;
using CarRental_BE.Repositories;
using CarRental_BE.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CarRental_BE.Services.Background.Email;

public sealed class StatusChangeEmailBackgroundService : BackgroundService
{
    private readonly IEmailJobQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<StatusChangeEmailBackgroundService> _logger;

    public StatusChangeEmailBackgroundService(
        IEmailJobQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<StatusChangeEmailBackgroundService> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StatusChangeEmailBackgroundService started.");
        while (!stoppingToken.IsCancellationRequested)
        {
            StatusChangeEmailJob job;
            try
            {
                job = await _queue.DequeueAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var bookingRepo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
                var carRepo = scope.ServiceProvider.GetRequiredService<ICarRepository>();
                var accountRepo = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var booking = await bookingRepo.GetByBookingNumberAsync(job.BookingNumber);
                if (booking == null)
                {
                    _logger.LogWarning("Email job skipped: booking {BookingNumber} not found.", job.BookingNumber);
                    continue;
                }

                var car = booking.Car ?? await carRepo.GetByIdAsync(booking.CarId);
                var owner = car?.OwnerAccountId != null ? await accountRepo.GetByIdAsync(car.OwnerAccountId) : null;
                var renter = await accountRepo.GetByIdAsync(booking.RenterAccountId);

                var subject = $"Booking {booking.BookingNumber} status updated: {job.NewStatus}";

                string BuildTable() => "<table style='border-collapse:collapse;font-size:14px;'>"
                    + $"<tr><td style='padding:4px;font-weight:bold;'>Previous Status:</td><td style='padding:4px;'>{job.OldStatus}</td></tr>"
                    + $"<tr><td style='padding:4px;font-weight:bold;'>New Status:</td><td style='padding:4px;'>{job.NewStatus}</td></tr>"
                    + $"<tr><td style='padding:4px;font-weight:bold;'>Pickup:</td><td style='padding:4px;'>{booking.PickUpTime:yyyy-MM-dd HH:mm}</td></tr>"
                    + $"<tr><td style='padding:4px;font-weight:bold;'>Dropoff:</td><td style='padding:4px;'>{booking.DropOffTime:yyyy-MM-dd HH:mm}</td></tr>"
                    + (job.Note != null
                        ? $"<tr><td style='padding:4px;font-weight:bold;'>Note:</td><td style='padding:4px;'>{WebUtility.HtmlEncode(job.Note)}</td></tr>"
                        : string.Empty)
                    + "</table>";

                var ownerBody =
                    "<h2>Booking Status Update</h2>"
                    + $"<p>Dear {(owner?.Email ?? "Owner")},</p>"
                    + $"<p>The booking <strong>{booking.BookingNumber}</strong> for <strong>{car?.Brand} {car?.Model}</strong> changed status.</p>"
                    + BuildTable()
                    + "<p>Please review the booking in your dashboard for any required action.</p>"
                    + "<p>Regards,<br/>Car Rental Platform</p>";

                var renterBody =
                    "<h2>Your Booking Status Changed</h2>"
                    + $"<p>Dear {(renter?.Email ?? "Customer")},</p>"
                    + $"<p>Your booking <strong>{booking.BookingNumber}</strong> for <strong>{car?.Brand} {car?.Model}</strong> has been updated.</p>"
                    + BuildTable()
                    + "<p>You can view full details and history in your account.</p>"
                    + "<p>Regards,<br/>Car Rental Platform</p>";

                var tasks = new List<Task>(2);
                if (!string.IsNullOrWhiteSpace(owner?.Email))
                    tasks.Add(emailService.SendEmailAsync(owner!.Email, subject, ownerBody));
                if (!string.IsNullOrWhiteSpace(renter?.Email))
                    tasks.Add(emailService.SendEmailAsync(renter!.Email, subject, renterBody));

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send status change emails for booking {BookingNumber}", job.BookingNumber);
            }
        }

        _logger.LogInformation("StatusChangeEmailBackgroundService stopped.");
    }
}