namespace CarRental_BE.Services.Background.Email;

public sealed class StatusChangeEmailJob
{
    public required string BookingNumber { get; init; }
    public required string OldStatus { get; init; }
    public required string NewStatus { get; init; }
    public string? Note { get; init; }
}