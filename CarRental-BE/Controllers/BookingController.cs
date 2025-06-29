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

        // Get bookings with pagination (admin view)
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginationResponse<BookingVO>>>> GetBookings(int page = 1, int pageSize = 5)
        {
            var (bookings, totalCount) = await _bookingService.GetBookingsWithPagingAsync(page, pageSize);
            var paginatedResponse = new PaginationResponse<BookingVO>(bookings, page, pageSize, totalCount);
            return Ok(new ApiResponse<PaginationResponse<BookingVO>>(200, "Success", paginatedResponse));
        }
        [HttpPut("{bookingNumber}/cancel")]
        //task,async bat dong bo
        public async Task<IActionResult> CancelBooking(string bookingNumber)
        {
            var result = await _bookingService.CancelBookingAsync(bookingNumber);
            if (!result.Success)
                return BadRequest(new ApiResponse<string>(400, result.Message));

            return Ok(new ApiResponse<string>(200, "Booking cancelled successfully"));
        }
        [HttpPut("{bookingNumber}/confirm-pickup")]
        public async Task<IActionResult> ConfirmPickup(string bookingNumber)
        {
            var result = await _bookingService.ConfirmPickupAsync(bookingNumber);
            if (!result.Success)
                return BadRequest(new ApiResponse<string>(400, result.Message));

            return Ok(new ApiResponse<string>(200, "Pick-up successfully"));
        }
        [HttpPut("{bookingNumber}/return")]
        public async Task<IActionResult> ReturnCar(string bookingNumber)
        {
            var result = await _bookingService.ReturnCarAsync(bookingNumber);
            if (!result.Success)
                return BadRequest(new ApiResponse<string>(400, result.Message));
            return Ok(new ApiResponse<string>(200, "Return processed successfully"));
        }


        [HttpGet("detail/{bookingNumber}")]
        public async Task<ActionResult<ApiResponse<BookingDetailVO>>> GetBookingById(string bookingNumber)
        {
            var data = await _bookingService.GetBookingByBookingIdAsync(bookingNumber);
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

            BookingInformationVO bookingInformationVO = new BookingInformationVO
            {
                Car = carDetails,
                User = userDetails
            };

            return Ok(new ApiResponse<BookingInformationVO>(200, "Success", bookingInformationVO));
        }
    }
}
