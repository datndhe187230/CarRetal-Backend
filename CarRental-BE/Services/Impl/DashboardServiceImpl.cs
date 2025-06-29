using CarRental_BE.Models.VO.Statistic;
using CarRental_BE.Repositories;

namespace CarRental_BE.Services.Impl
{
    public class DashboardServiceImpl : IDashboardService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITransactionRepository _transactionRepository;

        public DashboardServiceImpl(IBookingRepository bookingRepository, ITransactionRepository transactionRepository)
        {
            _bookingRepository = bookingRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<DashboardStatsVO> GetDashboardStatsAsync()
        {
            var totalRevenue = await _bookingRepository.GetTotalRevenueAsync();
            var activeBookings = await _bookingRepository.GetActiveBookingsCountAsync();
            var totalBookings = await _bookingRepository.GetTotalBookingsCountAsync();

            // Calculate fleet utilization (this would need car repository)
            var fleetUtilization = 78.5m; // Placeholder

            return new DashboardStatsVO
            {
                TotalRevenue = totalRevenue/10,
                ActiveBookings = activeBookings,
                TotalCustomers = totalBookings, // Simplified - would need customer count
                FleetUtilization = fleetUtilization,
                RevenueChange = "+20.1% from last month",
                BookingsChange = "+12% from last month",
                CustomersChange = "+8.2% from last month",
                UtilizationChange = "-2.1% from last month"
            };
        }

        public async Task<IEnumerable<MonthlyRevenueVO>> GetMonthlyRevenueAsync(int year)
        {
            var monthlyData = await _bookingRepository.GetMonthlyRevenueAsync(year);

            // Convert month numbers to names
            var monthNames = new[] { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            return monthlyData.Select(m => new MonthlyRevenueVO
            {
                Month = monthNames[int.Parse(m.Month)],
                Total = m.Total
            });
        }

        public async Task<IEnumerable<TopBookedVehicleVO>> GetTopBookedVehiclesAsync(int count = 5)
        {
            return await _bookingRepository.GetTopBookedVehiclesAsync(count);
        }

        public async Task<IEnumerable<TopPayingCustomerVO>> GetTopPayingCustomersAsync(int count = 5)
        {
            return await _transactionRepository.GetTopPayingCustomersAsync(count);
        }

        public async Task<IEnumerable<RecentBookingVO>> GetRecentBookingsAsync(int count = 10)
        {
            var recentBookings = await _bookingRepository.GetRecentBookingsAsync(count);

            return recentBookings.Select(b => new RecentBookingVO
            {
                BookingNumber = b.BookingNumber,
                CustomerName = b.DriverFullName ?? "Unknown",
                CustomerEmail = b.DriverEmail ?? "Unknown",
                CarName = $"{b.Car?.Brand} {b.Car?.Model}",
                Status = b.Status ?? "Unknown",
                Amount = (decimal)(b.BasePrice ?? 0),
                CreatedAt = b.CreatedAt ?? DateTime.MinValue
            });
        }

        public async Task<IEnumerable<BookingStatusCountVO>> GetBookingStatusCountsAsync()
        {
            return await _bookingRepository.GetBookingStatusCountsAsync();
        }

        public async Task<IEnumerable<TransactionTypeCountVO>> GetTransactionTypeCountsAsync()
        {
            return await _transactionRepository.GetTransactionTypeCountsAsync();
        }

        public async Task<IEnumerable<DailyTransactionVO>> GetDailyTransactionsAsync(DateTime startDate, DateTime endDate)
        {
            return await _transactionRepository.GetDailyTransactionsAsync(startDate, endDate);
        }
    }
}
