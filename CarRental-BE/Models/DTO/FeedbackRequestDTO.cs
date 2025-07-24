namespace CarRental_BE.Models.DTO
{
    public class FeedbackRequestDTO
    {
        public string BookingNumber { get; set; } = null!;

        public int? Rating { get; set; }

        public string? Comment { get; set; }
    }
}
