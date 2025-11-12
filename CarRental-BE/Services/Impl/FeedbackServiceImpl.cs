// Services/FeedbackService.cs
using CarRental_BE.Data;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.NewEntities;
using CarRental_BE.Repositories;
using System;
using System.Threading.Tasks;
using CarRental_BE.Exceptions;

namespace CarRental_BE.Services
{
    public class FeedbackServiceImpl : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackServiceImpl(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<ApiResponse<FeedbackResponseDTO>> SubmitFeedbackAsync(string userId, FeedbackRequestDTO request)
        {
            var booking = await _feedbackRepository.GetBookingAsync(request.BookingNumber);
            if (booking == null)
                return new ApiResponse<FeedbackResponseDTO>(400, "Booking not found", null);

            // Validate user if provided
            if (Guid.TryParse(userId, out var fromAccountId))
            {
                if (booking.RenterAccountId != fromAccountId)
                {
                    // Optional: enforce ownership
                    // return new ApiResponse<FeedbackResponseDTO>(401, "Unauthorized", null);
                }
            }

            if (request.Rating.HasValue && (request.Rating < 1 || request.Rating > 5))
                return new ApiResponse<FeedbackResponseDTO>(400, "Rating must be between 1 and 5", null);

            var toAccountId = booking.Car.OwnerAccountId;

            var feedback = new Review
            {
                ReviewId = Guid.NewGuid(),
                BookingNumber = request.BookingNumber,
                FromAccountId = Guid.TryParse(userId, out var parsed) ? parsed : booking.RenterAccountId,
                ToAccountId = toAccountId,
                Rating = (byte)(request.Rating ?? 0),
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _feedbackRepository.AddFeedbackAsync(feedback);

            return new ApiResponse<FeedbackResponseDTO>(200, "Feedback submitted successfully", new FeedbackResponseDTO { Success = true, Message = "Feedback submitted successfully" });
        }
    }
}