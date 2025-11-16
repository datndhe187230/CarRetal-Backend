using System;

namespace CarRental_BE.Models.VO.Booking
{
 public class BookingStatusHistoryVO
 {
 public string? OldStatus { get; set; }
 public string NewStatus { get; set; }
 public string? Note { get; set; }
 public string? PictureUrl { get; set; }
 public DateTime ChangedAt { get; set; }
 }
}
