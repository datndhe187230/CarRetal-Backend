using CarRental_BE.Data;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.AdminManagement;
using CarRental_BE.Models.VO.Car;
using CarRental_BE.Models.VO.Statistic;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<ApiResponse<DashboardStatsVO>>> GetDashboardStats()
        {
            try
            {
                var stats = await _dashboardService.GetDashboardStatsAsync();
                return Ok(new ApiResponse<DashboardStatsVO>(200, "Dashboard stats retrieved successfully", stats));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<DashboardStatsVO>(500, "An error occurred while retrieving dashboard stats", default));
            }
        }

        [HttpGet("revenue/monthly")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MonthlyRevenueVO>>>> GetMonthlyRevenue([FromQuery] int year = 0)
        {
            try
            {
                if (year == 0) year = DateTime.Now.Year;
                var revenue = await _dashboardService.GetMonthlyRevenueAsync(year);
                return Ok(new ApiResponse<IEnumerable<MonthlyRevenueVO>>(200, "Monthly revenue retrieved successfully", revenue));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<MonthlyRevenueVO>>(500, "An error occurred while retrieving monthly revenue", null));
            }
        }

        [HttpGet("vehicles/top-booked")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TopBookedVehicleVO>>>> GetTopBookedVehicles([FromQuery] int count = 5)
        {

            var vehicles = await _dashboardService.GetTopBookedVehiclesAsync(count);
            return Ok(new ApiResponse<IEnumerable<TopBookedVehicleVO>>(200, "Top booked vehicles retrieved successfully", vehicles));

        }

        [HttpGet("customers/top-paying")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TopPayingCustomerVO>>>> GetTopPayingCustomers([FromQuery] int count = 6)
        {
            var customers = await _dashboardService.GetTopPayingCustomersAsync(count);
            return Ok(new ApiResponse<IEnumerable<TopPayingCustomerVO>>(200, "Top paying customers retrieved successfully", customers));
        }

        [HttpGet("bookings/recent")]
        public async Task<ActionResult<ApiResponse<IEnumerable<RecentBookingVO>>>> GetRecentBookings([FromQuery] int count = 10)
        {
            try
            {
                var bookings = await _dashboardService.GetRecentBookingsAsync(count);
                return Ok(new ApiResponse<IEnumerable<RecentBookingVO>>(200, "Recent bookings retrieved successfully", bookings));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<RecentBookingVO>>(500, "An error occurred while retrieving recent bookings", null));
            }
        }

        [HttpGet("bookings/status-counts")]
        public async Task<ActionResult<ApiResponse<IEnumerable<BookingStatusCountVO>>>> GetBookingStatusCounts()
        {
            try
            {
                var statusCounts = await _dashboardService.GetBookingStatusCountsAsync();
                return Ok(new ApiResponse<IEnumerable<BookingStatusCountVO>>(200, "Booking status counts retrieved successfully", statusCounts));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<BookingStatusCountVO>>(500, "An error occurred while retrieving booking status counts", null));
            }
        }

        [HttpGet("transactions/type-counts")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TransactionTypeCountVO>>>> GetTransactionTypeCounts()
        {
            try
            {
                var typeCounts = await _dashboardService.GetTransactionTypeCountsAsync();
                return Ok(new ApiResponse<IEnumerable<TransactionTypeCountVO>>(200, "Transaction type counts retrieved successfully", typeCounts));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<TransactionTypeCountVO>>(500, "An error occurred while retrieving transaction type counts", null));
            }
        }

        [HttpGet("transactions/daily")]
        public async Task<ActionResult<ApiResponse<IEnumerable<DailyTransactionVO>>>> GetDailyTransactions(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                startDate ??= DateTime.Now.AddDays(-30);
                endDate ??= DateTime.Now;

                var transactions = await _dashboardService.GetDailyTransactionsAsync(startDate.Value, endDate.Value);
                return Ok(new ApiResponse<IEnumerable<DailyTransactionVO>>(200, "Daily transactions retrieved successfully", transactions));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<DailyTransactionVO>>(500, "An error occurred while retrieving daily transactions", null));
            }
        }

        [HttpGet("accounts/paginated")]
        public async Task<ActionResult<ApiResponse<PaginationResponse<AccountVO>>>> GetAccountsWithPaging(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginationRequest = new PaginationRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var paginatedAccounts = await _dashboardService.GetAccountsWithPagingAsync(paginationRequest);
            return Ok(new ApiResponse<PaginationResponse<AccountVO>>(200, "Paginated accounts retrieved successfully", paginatedAccounts));
        }

        [AllowAnonymous]
        [HttpGet("cars/unverified/paginated")]
        public async Task<ActionResult<ApiResponse<PaginationResponse<CarVO_Full>>>> GetAllUnverifiedCars(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? sortBy = null,
    [FromQuery] string? sortDirection = null,
    [FromQuery] string? brand = null,
    [FromQuery] string? search = null)
        {
            var filters = new CarFilterDTO
            {
                SortBy = sortBy,
                SortDirection = sortDirection,
                Brand = brand,
                Search = search
            };

            var paginationRequest = new PaginationRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var paginatedCars = await _dashboardService.GetAllUnverifiedCarsAsync(
                paginationRequest,
                filters);

            return Ok(new ApiResponse<PaginationResponse<CarVO_Full>>(
                200,
                "Paginated unverified cars retrieved successfully",
                paginatedCars));
        }

        [HttpPut("accounts/toggle-status/{accountId}")]
        public async Task<ActionResult<ApiResponse<string>>> ToggleAccountStatus(Guid accountId)
        {
            await _dashboardService.ToggleAccountStatus(accountId);
            return Ok(new ApiResponse<string>(200, "Account status toggled successfully", "Account status toggled successfully"));
        }

        [HttpPut("cars/toggle-verification/{carId}")]
        public async Task<ActionResult<ApiResponse<string>>> ToggleCarVerificationStatus(Guid carId)
        {

            await _dashboardService.ToggleCarVerificationStatus(carId);
            return Ok(new ApiResponse<string>(200, "Car verification status toggled successfully", "Car verification status toggled successfully"));
        }
        [HttpGet("cars/account/{accountId}/paginated")]
        public async Task<ActionResult<ApiResponse<PaginationResponse<CarVO_Full>>>> GetCarsByAccountId(
    Guid accountId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? sortBy = null,
    [FromQuery] string? sortDirection = null,
    [FromQuery] string? brand = null,
    [FromQuery] string? search = null
)
        {
            var filters = new CarFilterDTO
            {
                SortBy = sortBy,
                SortDirection = sortDirection,
                Brand = brand,
                Search = search
            };

            var paginationRequest = new PaginationRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var paginatedCars = await _dashboardService.GetFilteredCarsByAccountId(accountId, paginationRequest, filters);
            return Ok(new ApiResponse<PaginationResponse<CarVO_Full>>(200, "Paginated cars by account retrieved successfully", paginatedCars));
        }

    }
}
