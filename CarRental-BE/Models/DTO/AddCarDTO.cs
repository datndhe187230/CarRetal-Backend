namespace CarRental_BE.Models.DTO
{
    public class AddCarDTO
    {
        // Basic Info
        public string LicensePlate { get; set; } = null!;
        public string BrandName { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string ProductionYear { get; set; } = null!;
        public string Transmission { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string NumberOfSeats { get; set; } = null!;
        public string Fuel { get; set; } = null!;

        // Documents
        public IFormFile? RegistrationPaper { get; set; }
        public IFormFile? CertificateOfInspection { get; set; }
        public IFormFile? Insurance { get; set; }

        // Details
        public string Mileage { get; set; } = null!;
        public string FuelConsumption { get; set; } = null!;

        public string SearchAddress { get; set; } = null!;
        public string CityProvince { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Ward { get; set; } = null!;
        public string HouseNumber { get; set; } = null!;

        public string Description { get; set; } = string.Empty;

        // Additional Functions
        public bool Bluetooth { get; set; }
        public bool GPS { get; set; }
        public bool Camera { get; set; }
        public bool SunRoof { get; set; }
        public bool ChildLock { get; set; }
        public bool ChildSeat { get; set; }
        public bool DVD { get; set; }
        public bool USB { get; set; }

        // Images
        public IFormFile? FrontImage { get; set; }
        public IFormFile? BackImage { get; set; }
        public IFormFile? LeftImage { get; set; }
        public IFormFile? RightImage { get; set; }

        // Pricing
        public string BasePrice { get; set; } = null!;
        public string RequiredDeposit { get; set; } = null!;

        // Terms of Use
        public bool NoSmoking { get; set; }
        public bool NoFoodInCar { get; set; }
        public bool NoPet { get; set; }
        public bool Other { get; set; }
        public string OtherText { get; set; } = string.Empty;
    }
}
