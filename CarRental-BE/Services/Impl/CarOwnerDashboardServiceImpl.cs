using CarRental_BE.Models.VO.CarOwnerDashboard;
using CarRental_BE.Models.VO.Statistic;
using CarRental_BE.Repositories;

namespace CarRental_BE.Services.Impl
{
 public class CarOwnerDashboardServiceImpl : ICarOwnerDashboardService
 {
 private readonly IBookingRepository _bookingRepository;
 private readonly IFeedbackRepository _feedbackRepository;
 private readonly IAccountRepository _accountRepository;
 private readonly ICarRepository _carRepository;

 public CarOwnerDashboardServiceImpl(
 IBookingRepository bookingRepository,
 IFeedbackRepository feedbackRepository,
 IAccountRepository accountRepository,
 ICarRepository carRepository)
 {
 _bookingRepository = bookingRepository;
 _feedbackRepository = feedbackRepository;
 _accountRepository = accountRepository;
 _carRepository = carRepository;
 }

 public async Task<CarOwnerDashboardStatsVO> GetStatsAsync(Guid ownerAccountId)
 {
 var totalRevenue = await _bookingRepository.GetOwnerTotalRevenueAsync(ownerAccountId);
 var activeBookings = await _bookingRepository.GetOwnerActiveBookingsCountAsync(ownerAccountId);
 var totalCustomers = await _bookingRepository.GetOwnerTotalCustomersCountAsync(ownerAccountId);
 var fleetUtilization = await _bookingRepository.GetOwnerFleetUtilizationAsync(ownerAccountId);

 // Placeholder trend strings, could be computed from previous period deltas
 return new CarOwnerDashboardStatsVO
 {
 TotalRevenue = totalRevenue,
 ActiveBookings = activeBookings,
 TotalCustomers = totalCustomers,
 FleetUtilization = fleetUtilization,
 RevenueChange = "+0.0%",
 BookingsChange = "+0.0%",
 CustomersChange = "+0.0%",
 UtilizationChange = "+0.0%"
 };
 }

 public async Task<IEnumerable<CarOwnerMonthlyRevenueVO>> GetMonthlyRevenueAsync(Guid ownerAccountId, int year)
 {
 var items = await _bookingRepository.GetOwnerMonthlyRevenueAsync(ownerAccountId, year);
 var monthNames = new[] { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
 return items.Select(m => new CarOwnerMonthlyRevenueVO
 {
 Month = monthNames[int.Parse(m.Month)],
 Total = m.Total
 });
 }

 public async Task<CarOwnerRatingSummaryVO> GetRatingSummaryAsync(Guid ownerAccountId)
 {
 // Reuse feedback repository aggregate for owner's cars
 var summary = await _feedbackRepository.GetFeedbackSummaryByUserIdAsync(ownerAccountId);
 var distribution = new List<CarOwnerRatingDistributionItemVO>();
 int total = summary.TotalRatings;
 for (int stars =5; stars >=1; stars--)
 {
 summary.RatingDistribution.TryGetValue(stars, out var count);
 int percentage = total >0 ? (int)Math.Round((count *100.0) / total) :0;
 distribution.Add(new CarOwnerRatingDistributionItemVO
 {
 Stars = stars,
 Count = count,
 Percentage = percentage
 });
 }
 return new CarOwnerRatingSummaryVO
 {
 OverallRating = summary.AverageRating,
 TotalReviews = summary.TotalRatings,
 TrendChange = "+0.0%",
 Distribution = distribution
 };
 }

 public async Task<IEnumerable<CarOwnerRecentReviewVO>> GetRecentReviewsAsync(Guid ownerAccountId, int limit =3)
 {
 var request = new CarRental_BE.Models.Common.PaginationRequest
 {
 PageNumber =1,
 PageSize = limit
 };
 var items = await _feedbackRepository.GetFeedbackItemsByUserIdAsync(ownerAccountId, request);
 return items.Data.Select(i => new CarOwnerRecentReviewVO
 {
 Id = Guid.NewGuid().ToString(), // No id in DTO, generate ephemeral
 ReviewerName = i.CarName, // we don't have reviewer name in current DTO; placeholder
 Comment = i.Comment ?? string.Empty,
 Rating = i.Rating,
 CreatedAt = i.CreatedAt
 });
 }
 }
}
