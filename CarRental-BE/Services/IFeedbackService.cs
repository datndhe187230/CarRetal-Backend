using CarRental_BE.Data;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.NewEntities;


namespace CarRental_BE.Services
{
    public interface IFeedbackService
    {
        Task<ApiResponse<FeedbackResponseDTO>> SubmitFeedbackAsync(string userId, FeedbackRequestDTO request);
    }
}
