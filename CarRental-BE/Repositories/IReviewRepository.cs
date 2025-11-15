using CarRental_BE.Models;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;

namespace CarRental_BE.Repositories
{
    public interface IReviewRepository
    {
        Task<Review> AddFeedbackAsync(Review feedback);
        Task<Booking> GetBookingAsync(string bookingNumber);
        Task<FeedbackSummaryDTO> GetFeedbackSummaryByUserIdAsync(Guid userId);
        Task<PaginationResponse<FeedbackItemDTO>> GetFeedbackItemsByUserIdAsync(Guid userId, PaginationRequest request);
    }
}
