using CarRental_BE.Models.Common;

namespace CarRental_BE.Models.DTO
{
    public class FeedbackReportDTO
    {
        public FeedbackSummaryDTO Summary { get; set; } = null!;
        public PaginationResponse<FeedbackItemDTO> Details { get; set; } = null!;

    }
}
