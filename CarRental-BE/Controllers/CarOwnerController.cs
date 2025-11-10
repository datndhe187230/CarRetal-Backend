using CarRental_BE.Data;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.Enum;
using CarRental_BE.Models.VO.CarOwnerDashboard;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarRental_BE.Models.DTO;

namespace CarRental_BE.Controllers
{
    [ApiController]
    [Route("api/car-owner")]
    [Authorize(Roles = "car_owner")]
    public class CarOwnerController : ControllerBase
    {
        private readonly ICarOwnerDashboardService _service;

        public CarOwnerController(ICarOwnerDashboardService service)
        {
            _service = service;
        }

        // Dashboard endpoints
        [HttpGet("dashboard/stats")]
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

        [HttpGet("dashboard/revenue/monthly")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CarOwnerMonthlyRevenueVO>>>> GetMonthlyRevenue([FromQuery] int year)
        {
            var accountIdClaim = User.FindFirst("id")?.Value;
            if (!Guid.TryParse(accountIdClaim, out var accountId))
            {
                return Unauthorized(new ApiResponse<string>(401, "Unauthorized"));
            }
            if (year ==0) year = DateTime.UtcNow.Year;
            var data = await _service.GetMonthlyRevenueAsync(accountId, year);
            return Ok(new ApiResponse<IEnumerable<CarOwnerMonthlyRevenueVO>>(200, "success", data));
        }
        [HttpGet("dashboard/upcomming-bookings")]
        public async Task<ActionResult<ApiResponse<List<BookingVO>>>> GetUpcommingBookingsByAccountId(Guid accountId, int limit)
        {
            var data = await _service.GetUpcommingBookingsByAccountIdAsync(accountId, limit);
            return Ok(new ApiResponse<List<BookingVO>>(200, "Success", data));
        }
        [HttpGet("dashboard/{accountId}/ratings/summary")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<CarOwnerRatingSummaryVO>>> GetRatingsSummary(Guid accountId)
        {
            var data = await _service.GetRatingSummaryAsync(accountId);
            return Ok(new ApiResponse<CarOwnerRatingSummaryVO>(200, "success", data));
        }

        [HttpGet("dashboard/{accountId}/ratings/recent")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<CarOwnerRecentReviewVO>>>> GetRecentRatings(Guid accountId, [FromQuery] int limit =3)
        {
            var data = await _service.GetRecentReviewsAsync(accountId, limit);
            return Ok(new ApiResponse<IEnumerable<CarOwnerRecentReviewVO>>(200, "success", data));
        }

        // Simplified car owner bookings endpoint using DTO validation
        [HttpGet("bookings")]
        public async Task<ActionResult<ApiResponse<PaginationResponse<CarOwnerBookingListItemVO>>>> GetOwnerBookings([FromQuery] CarOwnerBookingListDTO query)
        {
            // Inject authenticated account id into DTO
            var principalIdStr = User.FindFirst("id")?.Value;
            if (!Guid.TryParse(principalIdStr, out var principalId))
            {
                return Unauthorized(new ApiResponse<string>(401, "Unauthorized"));
            }
            query.AccountId = query.AccountId == Guid.Empty ? principalId : query.AccountId;
            if (query.AccountId != principalId)
            {
                return StatusCode(403, new ApiResponse<string>(403, "Forbidden"));
            }

            // Normalize + validate
            query.Normalize();
            if (!query.TryValidate(out var errorPayload))
            {
                return BadRequest(errorPayload);
            }

            try
            {
                var result = await _service.GetOwnerBookingsAsync(principalId, query);
                if (!result.Data.Any())
                {
                    return NotFound(new ApiResponse<string>(404, "No bookings found"));
                }
                return Ok(new ApiResponse<PaginationResponse<CarOwnerBookingListItemVO>>(200, "Success", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { code =400, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<string>(500, "Internal server error"));
            }
        }

        // Earnings metrics
        [HttpGet("dashboard/earnings")]
        public async Task<ActionResult<ApiResponse<CarOwnerEarningsVO>>> GetEarnings()
        {
            var principalIdStr = User.FindFirst("id")?.Value;
            if (!Guid.TryParse(principalIdStr, out var accountId))
            {
                return Unauthorized(new ApiResponse<string>(401, "Unauthorized"));
            }
            try
            {
                var data = await _service.GetEarningsAsync(accountId);
                return Ok(new ApiResponse<CarOwnerEarningsVO>(200, "success", data));
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new ApiResponse<string>(403, "Forbidden"));
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<string>(500, "Internal server error"));
            }
        }

        // Fleet metrics
        [HttpGet("dashboard/fleet")]
        public async Task<ActionResult<ApiResponse<CarOwnerFleetVO>>> GetFleet()
        {
            var principalIdStr = User.FindFirst("id")?.Value;
            if (!Guid.TryParse(principalIdStr, out var accountId))
            {
                return Unauthorized(new ApiResponse<string>(401, "Unauthorized"));
            }
            try
            {
                var data = await _service.GetFleetAsync(accountId);
                return Ok(new ApiResponse<CarOwnerFleetVO>(200, "success", data));
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new ApiResponse<string>(403, "Forbidden"));
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<string>(500, "Internal server error"));
            }
        }
    }
}
