using CarRental_BE.Data;
using CarRental_BE.Models.DTO;

namespace CarRental_BE.Services
{
    public interface IFeedbackService
    {
        Task<ApiResponse<FeedbackResponseDTO>> SubmitFeedbackAsync(string userId, FeedbackRequestDTO request);
    }
}
