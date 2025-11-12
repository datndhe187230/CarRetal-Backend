using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.NewEntities;

public partial class UserProfile
{
    public Guid AccountId { get; set; }

    public string? FullName { get; set; }

    public DateOnly? Dob { get; set; }

    public string? PhoneNumber { get; set; }

    public string? NationalId { get; set; }

    public string? DrivingLicenseUri { get; set; }

    public bool? DrivingLicenseVerified { get; set; }

    public Guid? AddressId { get; set; }

    public decimal? AverageRenterRating { get; set; }

    public decimal? AverageOwnerRating { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Address? Address { get; set; }
}
