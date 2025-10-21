namespace CarRental_BE.Models.DTO
{
    public class FeedbackItemDTO
    {
        public string CarName { get; set; } = null!;
        public string CarImageUrl { get; set; } = null!;
        public string? Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PickUpTime { get; set; }
        public DateTime DropOffTime { get; set; }
    }
}
