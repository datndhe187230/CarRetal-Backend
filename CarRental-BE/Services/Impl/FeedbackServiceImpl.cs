// Services/FeedbackService.cs
using CarRental_BE.Data;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using CarRental_BE.Repositories;
using System;
using System.Threading.Tasks;

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

            //if (booking.AccountId.ToString() != userId)
                //return new ApiResponse<FeedbackResponseDTO>(401, "Unauthorized", null);


            if (request.Rating.HasValue && (request.Rating < 1 || request.Rating > 5))
                return new ApiResponse<FeedbackResponseDTO>(400, "Rating must be between 1 and 5", null);

            var feedback = new Feedback
            {
                BookingNumber = request.BookingNumber,
                Rating = request.Rating,
                Comment = request.Comment,
                CreateAt = DateTime.UtcNow, 
                UpdateAt = DateTime.UtcNow
            };

            await _feedbackRepository.AddFeedbackAsync(feedback);

            // Trả về phản hồi thành công
            return new ApiResponse<FeedbackResponseDTO>(200, "Feedback submitted successfully", new FeedbackResponseDTO { Success = true, Message = "Feedback submitted successfully" });
        }
    }
}