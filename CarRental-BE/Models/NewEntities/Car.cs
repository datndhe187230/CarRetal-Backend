using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.NewEntities;

public partial class Car
{
    public Guid CarId { get; set; }

    public Guid OwnerAccountId { get; set; }

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string LicensePlate { get; set; } = null!;

    public string? Color { get; set; }

    public byte NumberOfSeats { get; set; }

    public short ProductionYear { get; set; }

    public string FuelType { get; set; } = null!;

    public string Transmission { get; set; } = null!;

    public decimal? MileageKm { get; set; }

    public Guid? AddressId { get; set; }

    public string Status { get; set; } = null!;

    public decimal? AverageRating { get; set; }

    public int TotalRentals { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? TermOfUse { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<CarCalendar> CarCalendars { get; set; } = new List<CarCalendar>();

    public virtual ICollection<CarDocument> CarDocuments { get; set; } = new List<CarDocument>();

    public virtual ICollection<CarImage> CarImages { get; set; } = new List<CarImage>();

    public virtual ICollection<CarPricingPlan> CarPricingPlans { get; set; } = new List<CarPricingPlan>();

    public virtual Account OwnerAccount { get; set; } = null!;

    public virtual ICollection<CarFeature> Features { get; set; } = new List<CarFeature>();


}
