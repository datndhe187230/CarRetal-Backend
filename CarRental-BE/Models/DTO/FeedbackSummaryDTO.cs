namespace CarRental_BE.Models.DTO
{
    public class FeedbackSummaryDTO
    {
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
    }
}
