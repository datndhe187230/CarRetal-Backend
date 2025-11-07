using CarRental_BE.Models.VO.CarOwnerDashboard;

namespace CarRental_BE.Services
{
 public interface ICarOwnerDashboardService
 {
 Task<CarOwnerDashboardStatsVO> GetStatsAsync(Guid ownerAccountId);
 Task<IEnumerable<CarOwnerMonthlyRevenueVO>> GetMonthlyRevenueAsync(Guid ownerAccountId, int year);
 Task<CarOwnerRatingSummaryVO> GetRatingSummaryAsync(Guid ownerAccountId);
 Task<IEnumerable<CarOwnerRecentReviewVO>> GetRecentReviewsAsync(Guid ownerAccountId, int limit =3);
 }
}
