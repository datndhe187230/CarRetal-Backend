using CarRental_BE.Data;
using CarRental_BE.Models.VO.CarOwnerDashboard;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRental_BE.Controllers
{
    [ApiController]
    [Route("api/car-owner/dashboard")]
    [Authorize(Roles = "car_owner,admin")]
    public class CarOwnerDashboardController : ControllerBase
    {
        private readonly ICarOwnerDashboardService _service;

        public CarOwnerDashboardController(ICarOwnerDashboardService service)
        {
            _service = service;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<ApiResponse<CarOwnerDashboardStatsVO>>> GetStats()
        {
            var accountIdClaim = User.FindFirst("id")?.Value;
            if (!Guid.TryParse(accountIdClaim, out var accountId))
            {
                return Unauthorized(new ApiResponse<string>(401, "Unauthorized"));
            }
            var stats = await _service.GetStatsAsync(accountId);
            return Ok(new ApiResponse<CarOwnerDashboardStatsVO>(200, "success", stats));
        }

        [HttpGet("revenue/monthly")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CarOwnerMonthlyRevenueVO>>>> GetMonthlyRevenue([FromQuery] int year)
        {
            var accountIdClaim = User.FindFirst("id")?.Value;
            if (!Guid.TryParse(accountIdClaim, out var accountId))
            {
                return Unauthorized(new ApiResponse<string>(401, "Unauthorized"));
            }
            if (year == 0) year = DateTime.UtcNow.Year;
            var data = await _service.GetMonthlyRevenueAsync(accountId, year);
            return Ok(new ApiResponse<IEnumerable<CarOwnerMonthlyRevenueVO>>(200, "success", data));
        }

        [HttpGet("{accountId}/ratings/summary")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<CarOwnerRatingSummaryVO>>> GetRatingsSummary(Guid accountId)
        {
            var data = await _service.GetRatingSummaryAsync(accountId);
            return Ok(new ApiResponse<CarOwnerRatingSummaryVO>(200, "success", data));
        }

        [HttpGet("{accountId}/ratings/recent")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<CarOwnerRecentReviewVO>>>> GetRecentRatings(Guid accountId, [FromQuery] int limit = 3)
        {
            var data = await _service.GetRecentReviewsAsync(accountId, limit);
            return Ok(new ApiResponse<IEnumerable<CarOwnerRecentReviewVO>>(200, "success", data));
        }
    }
}
