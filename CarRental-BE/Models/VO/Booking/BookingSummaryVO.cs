using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.VO.Booking
{
 public class BookingSummaryVO
 {
 public string BookingNumber { get; set; } = string.Empty;
 public string Status { get; set; } = string.Empty;

 // Times
 public DateTime PickUpTime { get; set; }
 public DateTime DropOffTime { get; set; }
 public DateTime? ActualReturnTime { get; set; }

 // Pricing snapshot
 public decimal BasePricePerDayCents { get; set; }
 public int TotalDays { get; set; }
 public decimal BasePriceCents { get; set; }
 public decimal? DepositSnapshotCents { get; set; }

 // Breakdown
 public decimal ExtraKmFeeCents { get; set; }
 public decimal DiscountCents { get; set; }
 public decimal ExtraChargesCents { get; set; }

 // Totals and settlement
 public decimal TotalCalculatedCents { get; set; }
 public decimal RemainingChargedCents { get; set; }
 public decimal RefundToRenterCents { get; set; }

 // Revenue shares
 public decimal OwnerShareFromDepositCents { get; set; }
 public decimal AdminShareFromDepositCents { get; set; }
 public decimal OwnerShareFromRemainingCents { get; set; }
 public decimal AdminShareFromRemainingCents { get; set; }

 // Timeline
 public List<BookingStatusHistoryVO>? Timeline { get; set; }
 }
}
