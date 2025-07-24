namespace CarRental_BE.Models.DTO
{
    public class CarSummaryDTO
    {
        public Guid Id { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Color { get; set; }
        public long BasePrice { get; set; }
        public long Deposit { get; set; }
        public string Status { get; set; } = null!;
        public string Address { get; set; } = null!;
        public double? AverageRating { get; set; }
    }

}
