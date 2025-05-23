using System;
using System.Collections.Generic;

namespace CarRental_BE.Models;

public partial class Booking
{
    public string BookingNumber { get; set; } = null!;

    public Guid CarId { get; set; }

    public string? DriverFullName { get; set; }

    public DateOnly? DriverDob { get; set; }

    public string? DriverEmail { get; set; }

    public string? DriverPhoneNumber { get; set; }

    public string? DriverNationalId { get; set; }

    public string? DriverDrivingLicenseUri { get; set; }

    public string? DriverHouseNumberStreet { get; set; }

    public string? DriverWard { get; set; }

    public string? DriverDistrict { get; set; }

    public string? DriverCityProvince { get; set; }

    public long? BasePrice { get; set; }

    public long? Deposit { get; set; }

    public string? PickUpLocation { get; set; }

    public string? DropOffLocation { get; set; }

    public DateTime? PickUpTime { get; set; }

    public DateTime? DropOffTime { get; set; }

    public string? PaymentType { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? AccountId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Car Car { get; set; } = null!;

    public virtual Feedback? Feedback { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
