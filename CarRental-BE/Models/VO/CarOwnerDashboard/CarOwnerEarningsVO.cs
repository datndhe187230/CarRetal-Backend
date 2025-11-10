namespace CarRental_BE.Models.VO.CarOwnerDashboard
{
 public class CarOwnerEarningsVO
 {
 public long? TotalRevenue { get; set; }
 public string RevenueChange { get; set; } = string.Empty;
 public long? NetProfit { get; set; }
 public string NetProfitChange { get; set; } = string.Empty;
 public long? PendingPayouts { get; set; }
 public long? AverageBookingValue { get; set; }
 public string AverageBookingValueChange { get; set; } = string.Empty;
 public int? CompletedBookingsThisMonth { get; set; }
 public List<CarOwnerMonthlyRevenuePoint> MonthlyRevenue { get; set; } = new();
 }

 public class CarOwnerMonthlyRevenuePoint
 {
 public string Month { get; set; } = string.Empty; // e.g.2025-01
 public long Total { get; set; }
 }

 public class CarOwnerFleetVO
 {
 public decimal? FleetUtilization { get; set; }
 public string UtilizationChange { get; set; } = string.Empty;
 public int? TotalBookings { get; set; }
 public string BookingsChange { get; set; } = string.Empty;
 public double? AverageBookingDurationDays { get; set; }
 public string AverageBookingDurationChange { get; set; } = string.Empty;
 public int? UpcomingBookings { get; set; }
 public decimal? CancellationRate { get; set; }
 public string CancellationRateChange { get; set; } = string.Empty;
 public int? ActiveFleet { get; set; }
 public int? InactiveFleet { get; set; }
 public int? FleetSize { get; set; }
 }
}
