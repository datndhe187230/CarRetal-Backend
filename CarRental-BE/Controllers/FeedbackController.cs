// Controllers/FeedbackController.cs
using CarRental_BE.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CarRental_BE.Services;
using CarRental_BE.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace CarRental_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ILogger<FeedbackController> _logger;
        private readonly CarRentalContext _context;

        public FeedbackController(IFeedbackService feedbackService, ILogger<FeedbackController> logger, CarRentalContext context)
        {
            _feedbackService = feedbackService;
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context)); 
        }

        [HttpPost("submit")]
        [AllowAnonymous] 
        public async Task<IActionResult> SubmitFeedback([FromBody] FeedbackRequestDTO request)
        {
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            _logger.LogInformation($"Feedback submission received at {vietnamTime} for booking {request.BookingNumber}");

            var response = await _feedbackService.SubmitFeedbackAsync(null, request); 
            return StatusCode(response.Status, response);
        }

    }
}