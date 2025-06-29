namespace CarRental_BE.Models.VO.Car
{
    public record CarSearchVO
    {
        public Guid Id { get; init; }
        public string Brand { get; init; } = string.Empty;
        public string Model { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public double Rating { get; init; }
        public DateTime BookedTime { get; init; }
        public long BasePrice { get; init; }
        public List<string> Images { get; init; } = new();
        public SpecsVO Specs { get; init; } = null!;
        public string Ward { get; init; } = string.Empty;
        public string District { get; init; } = string.Empty;
        public string CityProvince { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
    }

    public record SpecsVO
    {
        public string Engine { get; init; } = string.Empty;
        public string Fuel { get; init; } = string.Empty;
        public string Transmission { get; init; } = string.Empty;
        public string NumberOfSeat { get; init; } = string.Empty;
        public string ProductionYear { get; init; } = string.Empty;
        public string Mileage { get; init; } = string.Empty;
        public string FuelConsumption { get; init; } = string.Empty;
        public string Color { get; init; } = string.Empty;
    }
}