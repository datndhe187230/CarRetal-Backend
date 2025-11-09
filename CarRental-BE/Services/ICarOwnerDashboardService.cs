using CarRental_BE.Models.VO.CarOwnerDashboard;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;

namespace CarRental_BE.Services
{
    public interface ICarOwnerDashboardService
    {
        Task<CarOwnerDashboardStatsVO> GetStatsAsync(Guid ownerAccountId);
        Task<IEnumerable<CarOwnerMonthlyRevenueVO>> GetMonthlyRevenueAsync(Guid ownerAccountId, int year);
        Task<CarOwnerRatingSummaryVO> GetRatingSummaryAsync(Guid ownerAccountId);
        Task<IEnumerable<CarOwnerRecentReviewVO>> GetRecentReviewsAsync(Guid ownerAccountId, int limit = 3);
        Task<PaginationResponse<CarOwnerBookingListItemVO>> GetOwnerBookingsAsync(Guid ownerAccountId, CarOwnerBookingListDTO query);
    }
}
