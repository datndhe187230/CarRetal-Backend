using System;

namespace CarRental_BE.Models.DTO
{
 public record ConfirmBookingRequest(string BookingNumber);
 public record ConfirmDepositRequest(string BookingNumber);
 public record ConfirmPickupRequest(string BookingNumber);
 public record RequestReturnRequest(string BookingNumber);
 public record AcceptReturnRequest(string BookingNumber, string? Note, string? PictureUrl, long? ChargesCents);
 public record RejectReturnRequest(string BookingNumber, string? Note, string? PictureUrl);
 public record CancelBookingRequest(string BookingNumber, string? Reason, string? PictureUrl);
}
