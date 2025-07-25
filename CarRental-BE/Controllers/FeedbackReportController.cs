using CarRental_BE.Data;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class FeedbackReportController : ControllerBase
    {
        private readonly IFeedbackReportService _feedbackReportService;

        public FeedbackReportController(IFeedbackReportService feedbackReportService)
        {
            _feedbackReportService = feedbackReportService;
        }

        [HttpGet("{accountId}/paginated")]
        //[Authorize(Roles = "car_owner")]
        public async Task<ApiResponse<FeedbackReportDTO>> GetFeedbackReport(Guid accountId, [FromQuery] PaginationRequest request)
        {
            try
            {
                var result = await _feedbackReportService.GetFeedbackReportByUserIdAsync(accountId, request);

                if (result == null || (result.Details?.Data?.Count ?? 0) == 0)
                {
                    return new ApiResponse<FeedbackReportDTO>(
                        status: 404,
                        message: "No feedback report found for this user.",
                        data: null);
                }

                return new ApiResponse<FeedbackReportDTO>(
                    status: 200,
                    message: "Feedback report retrieved successfully.",
                    data: result);
            }
            catch (Exception ex)
            {
                return new ApiResponse<FeedbackReportDTO>(
                    status: 500,
                    message: $"An error occurred: {ex.Message}",
                    data: null
                );
            }
        }
    }
}
