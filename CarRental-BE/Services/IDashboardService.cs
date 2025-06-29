using CarRental_BE.Models.VO.Statistic;

namespace CarRental_BE.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsVO> GetDashboardStatsAsync();
        Task<IEnumerable<MonthlyRevenueVO>> GetMonthlyRevenueAsync(int year);
        Task<IEnumerable<TopBookedVehicleVO>> GetTopBookedVehiclesAsync(int count = 5);
        Task<IEnumerable<TopPayingCustomerVO>> GetTopPayingCustomersAsync(int count = 5);
        Task<IEnumerable<RecentBookingVO>> GetRecentBookingsAsync(int count = 10);
        Task<IEnumerable<BookingStatusCountVO>> GetBookingStatusCountsAsync();
        Task<IEnumerable<TransactionTypeCountVO>> GetTransactionTypeCountsAsync();
        Task<IEnumerable<DailyTransactionVO>> GetDailyTransactionsAsync(DateTime startDate, DateTime endDate);
    }
}
