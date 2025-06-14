namespace CarRental_BE.Models.VO.Car
{
    public record CarSearchVO
    {
        public int Id { get; init; }
        public string Brand { get; init; } = null!;
        public string Model { get; init; } = null!;
        public string Type { get; init; } = null!;
        public double Rating { get; init; }
        public int Reviews { get; init; }
        public DateTime BookedTime { get; init; }
        public decimal OriginalPrice { get; init; }
        public decimal DiscountedPrice { get; init; }
        public decimal DailyPrice { get; init; }
        public List<string> Images { get; init; } = new();
        public SpecsVO Specs { get; init; } = null!;
    }

    public record SpecsVO
    {
        public string Engine { get; init; } = null!;
        public string Fuel { get; init; } = null!;
        public string Transmission { get; init; } = null!;
        public string Efficiency { get; init; } = null!;
        public string Capacity { get; init; } = null!;
    }
}
