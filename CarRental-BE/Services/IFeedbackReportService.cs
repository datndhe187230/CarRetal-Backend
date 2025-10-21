using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;

namespace CarRental_BE.Services
{
    public interface IFeedbackReportService
    {
        Task<FeedbackReportDTO> GetFeedbackReportByUserIdAsync(Guid userId, PaginationRequest request);

    }
}
