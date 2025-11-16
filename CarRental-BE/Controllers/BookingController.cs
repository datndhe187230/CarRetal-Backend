using CarRental_BE.Data;
using CarRental_BE.Exceptions;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO;
using CarRental_BE.Models.VO.Booking;
using CarRental_BE.Models.VO.Car;
using CarRental_BE.Models.VO.User;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ICarService _carService;
        private readonly IUserService _userService;

        public BookingController(IBookingService bookingService, ICarService carService, IUserService userService)
        {
            _bookingService = bookingService;
            _carService = carService;
            _userService = userService;
        }

        // Get all bookings (for admin use or debugging)
        [HttpGet("all")]
        public async Task<ActionResult<ApiResponse<List<BookingVO>>>> GetAllBookings()
        {
            var data = await _bookingService.GetAllBookingsAsync();
            return Ok(new ApiResponse<List<BookingVO>>(200, "Success", data));
        }

        // Get bookings by account ID (user-specific)
        [HttpGet("{accountId}")]
        public async Task<ActionResult<ApiResponse<List<BookingVO>>>> GetBookingsByAccountId(Guid accountId)
        {
            var data = await _bookingService.GetBookingsByAccountIdAsync(accountId);
            return Ok(new ApiResponse<List<BookingVO>>(200, "Success", data));
        }

        // Get bookings by account ID with filter and sort
        [HttpGet("{accountId}/search")]
        public async Task<ActionResult<ApiResponse<List<BookingVO>>>> GetBookingsByAccountIdWithFilter(
            Guid accountId,
            [FromQuery] BookingQueryDto queryDto)
        {
            var data = await _bookingService.GetBookingsByAccountIdAsync(accountId, queryDto);
            return Ok(new ApiResponse<List<BookingVO>>(200, "Success", data));
        }

        // Get bookings with pagination (admin view)
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginationResponse<BookingVO>>>> GetBookings(int page = 1, int pageSize = 5)
        {
            var (bookings, totalCount) = await _bookingService.GetBookingsWithPagingAsync(page, pageSize);
            var paginatedResponse = new PaginationResponse<BookingVO>(bookings, page, pageSize, totalCount);
            return Ok(new ApiResponse<PaginationResponse<BookingVO>>(200, "Success", paginatedResponse));
        }

        [HttpPut("{bookingNumber}/customer-cancel")]
        public async Task<IActionResult> CancelBooking(string bookingNumber)
        {
            var result = await _bookingService.CustomerCancelAsync(bookingNumber);
            if (!result.Success)
                return BadRequest(new ApiResponse<string>(400, result.Message));

            return Ok(new ApiResponse<string>(200, "Booking cancelled successfully"));
        }

        [HttpPut("{bookingNumber}/owner-cancel")]
        [Authorize(Roles = "car_owner")]
        public async Task<IActionResult> CarOwnerCancelBooking(string bookingNumber)
        {
            var result = await _bookingService.OwnerCancelAsync(bookingNumber);
            if (!result.Success)
                return BadRequest(new ApiResponse<string>(400, result.Message));

            return Ok(new ApiResponse<string>(200, "Booking cancelled successfully"));
        }

        [HttpPost("{bookingNumber}/confirm-pickup")]
        public async Task<IActionResult> ConfirmPickup(string bookingNumber)
        {
            var result = await _bookingService.ConfirmPickupAsync(bookingNumber);
            if (!result.Success)
                return BadRequest(new ApiResponse<string>(400, result.Message));

            return Ok(new ApiResponse<string>(200, "Pick-up successfully"));
        }

        // owner confirms booking
        [HttpPost("{bookingNumber}/confirm")]
        [Authorize(Roles = "car_owner")]
        public async Task<IActionResult> ConfirmBookingFlow(string bookingNumber)
        {
            var result = await _bookingService.ConfirmBookingAsync(bookingNumber);
            if (!result.Success)
                return BadRequest(new ApiResponse<string>(400, result.Message));

            return Ok(new ApiResponse<string>(200, "Confirmed"));
        }

        // customer requests return
        [HttpPost("{bookingNumber}/request-return")]
        public async Task<IActionResult> RequestReturn(string bookingNumber)
        {
            var result = await _bookingService.RequestReturnAsync(bookingNumber);
            if (!result.Success)
                return BadRequest(new ApiResponse<string>(400, result.Message));

            return Ok(new ApiResponse<string>(200, "Return requested"));
        }

        // owner accepts return
        [HttpPost("{bookingNumber}/accept-return")]
        [Authorize(Roles = "car_owner")]
        public async Task<IActionResult> AcceptReturn(string bookingNumber, [FromBody] AcceptReturnRequest req)
        {
            var result = await _bookingService.AcceptReturnAsync(bookingNumber, req?.Note, req?.PictureUrl, req?.ChargesCents);
            if (!result.Success)
                return BadRequest(new ApiResponse<string>(400, result.Message));

            return Ok(new ApiResponse<string>(200, "Return accepted"));
        }

        // owner rejects return
        [HttpPost("{bookingNumber}/reject-return")]
        [Authorize(Roles = "car_owner")]
        public async Task<IActionResult> RejectReturn(string bookingNumber, [FromBody] RejectReturnRequest req)
        {
            var result = await _bookingService.RejectReturnAsync(bookingNumber, req?.Note, req?.PictureUrl);
            if (!result.Success)
                return BadRequest(new ApiResponse<string>(400, result.Message));

            return Ok(new ApiResponse<string>(200, "Return rejected"));
        }

        // customer cancels
        [HttpPost("{bookingNumber}/customer-cancel")]
        public async Task<IActionResult> CustomerCancel(string bookingNumber, [FromBody] CancelBookingRequest req)
        {
            var result = await _bookingService.CustomerCancelAsync(bookingNumber, req?.Reason, req?.PictureUrl);
            if (!result.Success)
                return BadRequest(new ApiResponse<string>(400, result.Message));

            return Ok(new ApiResponse<string>(200, "Cancelled"));
        }

        // owner cancels
        [HttpPost("{bookingNumber}/owner-cancel")]
        [Authorize(Roles = "car_owner")]
        public async Task<IActionResult> OwnerCancel(string bookingNumber, [FromBody] CancelBookingRequest req)
        {
            var result = await _bookingService.OwnerCancelAsync(bookingNumber, req?.Reason, req?.PictureUrl);
            if (!result.Success)
                return BadRequest(new ApiResponse<string>(400, result.Message));

            return Ok(new ApiResponse<string>(200, "Cancelled"));
        }

        [HttpGet("detail/{bookingNumber}")]
        public async Task<ActionResult<ApiResponse<BookingDetailVO>>> GetBookingById(string bookingNumber)
        {
            var data = await _bookingService.GetBookingByBookingNumberAsync(bookingNumber);
            if (data == null)
                return NotFound(new ApiResponse<BookingVO>(404, "Booking detail not found", null));

            return Ok(new ApiResponse<BookingDetailVO>(200, "Success", data));
        }

        [HttpPut("edit/{bookingNumber}")]
        public async Task<ActionResult<ApiResponse<BookingDetailVO>>> UpdateBooking(string bookingNumber, [FromBody] BookingEditDTO bookingDto)
        {
            var updatedBooking = await _bookingService.UpdateBookingAsync(bookingNumber, bookingDto);
            return Ok(new ApiResponse<BookingDetailVO>(200, "Success", updatedBooking));
        }

        [Authorize]
        [HttpGet("booking-informations/{carId}")]
        public async Task<ActionResult<ApiResponse<BookingInformationVO>>> GetBookingInformationsByCarId(Guid carId)
        {
            if (!Guid.TryParse(User.FindFirst("id")?.Value, out Guid userId))
            {
                throw new UserNotFoundException();
            }

            var carDetails = await _carService.GetCarDetailById(carId);
            var userDetails = await _userService.GetUserProfile(userId);
            var occupiedDates = await _bookingService.GetOccupiedDatesByCarId(carId);

            if (occupiedDates == null)
            {
                occupiedDates = Array.Empty<OccupiedDateRange>();
            }

            if (userDetails == null)
            {
                return NotFound(new ApiResponse<BookingInformationVO>(404, "User details not found", null));
            }

            BookingInformationVO bookingInformationVO = new BookingInformationVO
            {
                Car = carDetails,
                User = userDetails,
                CarCallendar = occupiedDates
            };

            return Ok(new ApiResponse<BookingInformationVO>(200, "Success", bookingInformationVO));
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<BookingVO>>> CreateBooking([FromBody] BookingCreateDTO bookingCreateDto)
        {
            if (!Guid.TryParse(User.FindFirst("id")?.Value, out Guid userId))
            {
                throw new UserNotFoundException();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (bookingCreateDto == null)
            {
                return BadRequest(new ApiResponse<BookingVO>(400, "Invalid booking data", null));
            }

            BookingVO createdBooking = await _bookingService.CreateBookingAsync(userId, bookingCreateDto);
            return new ApiResponse<BookingVO>(201, "Booking created successfully", createdBooking);
        }

        [HttpPost("{bookingNumber}/confirm-deposit")]
        [Authorize(Roles = "car_owner")]
        public async Task<IActionResult> ConfirmDeposit(string bookingNumber)
        {
            var result = await _bookingService.ConfirmDepositAsync(bookingNumber);
            if (!result.Success)
                return BadRequest(new ApiResponse<string>(400, result.Message));

            return Ok(new ApiResponse<string>(200, result.Message));
        }

        [AllowAnonymous]
        [HttpGet("booking-details/{carId}")]
        public async Task<ActionResult<ApiResponse<BookingDetailVO>>> GetBookingDetailsByCarId(Guid carId)
        {
            var bookingDetail = await _bookingService.GetBookingInformationByCarId(carId);

            if (bookingDetail == null)
                return NotFound(new ApiResponse<BookingDetailVO>(404, "No pending deposit booking found", null));

            return Ok(new ApiResponse<BookingDetailVO>(200, "Success", bookingDetail));
        }

        [AllowAnonymous]
        [HttpGet("booking-details/batch")]
        public async Task<ActionResult<ApiResponse<Dictionary<string, BookingDetailVO>>>> GetBookingDetailsByCarIds([FromQuery] string[] carIds)
        {
            if (carIds == null || carIds.Length == 0)
                return BadRequest(new ApiResponse<string>(400, "No car IDs provided", null));

            var result = new Dictionary<string, BookingDetailVO>();

            foreach (var carIdStr in carIds)
            {
                if (!Guid.TryParse(carIdStr, out var carId))
                {
                    result[carIdStr] = null;
                    continue;
                }

                var bookingDetail = await _bookingService.GetBookingInformationByCarId(carId);
                result[carIdStr] = bookingDetail;
            }

            return Ok(new ApiResponse<Dictionary<string, BookingDetailVO>>(200, "Success", result));
        }

        [HttpGet("summary/{bookingNumber}")]
        public async Task<ActionResult<ApiResponse<BookingSummaryVO>>> GetBookingSummary(string bookingNumber)
        {
            var data = await _bookingService.GetBookingSummaryAsync(bookingNumber);
            if (data == null)
                return NotFound(new ApiResponse<string>(404, "Booking not found"));

            return Ok(new ApiResponse<BookingSummaryVO>(200, "Success", data));
        }
    }
}
