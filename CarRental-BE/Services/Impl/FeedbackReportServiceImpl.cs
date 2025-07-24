using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Repositories;

namespace CarRental_BE.Services.Impl
{
    public class FeedbackReportServiceImpl : IFeedbackReportService
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackReportServiceImpl(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<FeedbackReportDTO> GetFeedbackReportByUserIdAsync(Guid userId, PaginationRequest request)
        {
            var summary = await _feedbackRepository.GetFeedbackSummaryByUserIdAsync(userId);
            var details = await _feedbackRepository.GetFeedbackItemsByUserIdAsync(userId, request);

            return new FeedbackReportDTO
            {
                Summary = summary,
                Details = details
            };
        }
    }
}
