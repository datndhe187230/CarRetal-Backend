namespace CarRental_BE.Models.VO.CarOwnerDashboard
{
 public class CarOwnerDashboardStatsVO
 {
 public decimal TotalRevenue { get; set; }
 public int ActiveBookings { get; set; }
 public int TotalCustomers { get; set; }
 public decimal FleetUtilization { get; set; }
 public string RevenueChange { get; set; } = string.Empty;
 public string BookingsChange { get; set; } = string.Empty;
 public string CustomersChange { get; set; } = string.Empty;
 public string UtilizationChange { get; set; } = string.Empty;
 }

 public class CarOwnerMonthlyRevenueVO
 {
 public string Month { get; set; } = string.Empty;
 public decimal Total { get; set; }
 }

 public class CarOwnerRatingDistributionItemVO
 {
 public int Stars { get; set; }
 public int Count { get; set; }
 public int Percentage { get; set; }
 }

 public class CarOwnerRatingSummaryVO
 {
 public double OverallRating { get; set; }
 public int TotalReviews { get; set; }
 public string TrendChange { get; set; } = string.Empty;
 public List<CarOwnerRatingDistributionItemVO> Distribution { get; set; } = new();
 }

 public class CarOwnerRecentReviewVO
 {
 public string Id { get; set; } = string.Empty;
 public string ReviewerName { get; set; } = string.Empty;
 public string Comment { get; set; } = string.Empty;
 public int Rating { get; set; }
 public DateTime CreatedAt { get; set; }
 }
}
