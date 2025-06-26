namespace CarRental_BE.Models.VO.Statistic
{
    public class TopBookedVehicleVO
    {
        public Guid CarId { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int? Year { get; set; }
        public int TotalBookings { get; set; }
        public decimal Revenue { get; set; }
        public decimal UtilizationRate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Trend { get; set; } = string.Empty;
    }
}
