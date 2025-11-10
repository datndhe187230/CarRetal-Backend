namespace CarRental_BE.Models.VO.CarOwnerDashboard
{
 public class CarOwnerBookingListItemVO
 {
 public string BookingNumber { get; set; } = string.Empty;
 public string BookingId { get; set; } = string.Empty; // mirror bookingNumber or internal id if any
 public Guid CarId { get; set; }
 public string CarName { get; set; } = string.Empty;
 public string? CarImageFront { get; set; }
 public string Status { get; set; } = string.Empty;
 public DateTime? PickupDate { get; set; }
 public DateTime? ReturnDate { get; set; }
 public string? PickUpLocation { get; set; }
 public string? DropOffLocation { get; set; }
 public long? BasePrice { get; set; }
 public long? Deposit { get; set; }
 public long? TotalAmount { get; set; }
 public string? PaymentType { get; set; }
 public string? PaymentStatus { get; set; }
 public DateTime? CreatedAt { get; set; }
 public DateTime? UpdatedAt { get; set; }
 public string? RenterFullName { get; set; }
 public string? RenterEmail { get; set; }
 public string? RenterPhoneNumber { get; set; }
 }
}
