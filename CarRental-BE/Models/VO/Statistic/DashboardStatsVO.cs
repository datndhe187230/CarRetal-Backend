namespace CarRental_BE.Models.VO.Statistic
{
    public class DashboardStatsVO
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
}
