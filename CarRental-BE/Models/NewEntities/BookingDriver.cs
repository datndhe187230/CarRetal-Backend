using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.NewEntities;

public partial class BookingDriver
{
    public Guid BookingDriverId { get; set; }

    public string BookingNumber { get; set; } = null!;

    public Guid? AccountId { get; set; }

    public string? FullName { get; set; }

    public DateOnly? Dob { get; set; }

    public string? PhoneNumber { get; set; }

    public string? NationalId { get; set; }

    public string? DrivingLicenseUri { get; set; }

    public Guid? AddressId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Address? Address { get; set; }

    public virtual Booking BookingNumberNavigation { get; set; } = null!;
}
