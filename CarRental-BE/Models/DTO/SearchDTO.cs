using System.ComponentModel.DataAnnotations;

namespace CarRental_BE.Models.DTO
{
    public record SearchDTO
    {
        [Required]
        public string LocationProvince { get; set; }
        public string? LocationDistrict { get; set; } // Optional
        public string? LocationWard { get; set; } // Optional

        public DateTime? PickupTime { get; init; }
        public DateTime? DropoffTime { get; init; }

        public int PriceRangeMin { get; set; } = 0; // Default to 0 if not provided
        public int PriceRangeMax { get; set; } = int.MaxValue; // Default to max if not provided

        public string[]? CarTypes { get; set; } // Optional, comma-separated
        public string[]? FuelTypes { get; set; } // Optional, comma-separated
        public string[]? TransmissionTypes { get; set; } // Optional, comma-separated
        public string[]? Brands { get; set; } // Optional, comma-separated
        public string[]? Seats { get; set; } // Optional, comma-separated

        public string? SearchQuery { get; set; } // Optional

    }

}
