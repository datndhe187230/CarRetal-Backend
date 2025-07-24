using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.AdminManagement;
using CarRental_BE.Models.VO.Car;
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
        Task<PaginationResponse<AccountVO>> GetAccountsWithPagingAsync(PaginationRequest paginationRequest);
        Task<PaginationResponse<CarVO_Full>> GetAllUnverifiedCarsAsync(
       PaginationRequest paginationRequest,
       CarFilterDTO? filters = null); // Thêm tham số filters
        Task ToggleAccountStatus(Guid accountId);
        Task ToggleCarVerificationStatus(Guid carId);
        Task<PaginationResponse<CarVO_Full>> GetCarsByAccountIdAsync(Guid accountId, PaginationRequest paginationRequest);
        Task<PaginationResponse<CarVO_Full>> GetFilteredCarsByAccountId(Guid accountId, PaginationRequest paginationRequest, CarFilterDTO filters);

    }
}
