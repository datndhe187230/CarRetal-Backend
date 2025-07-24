using CarRental_BE.Models.Common;
using System.ComponentModel.DataAnnotations;

public class BookingCreateDTO
{
    [Required]

    public Guid CarId { get; set; }
    public Guid DriverId { get; set; }
    [Required]
    public DateTime PickupDate { get; set; }
    [Required]
    public DateTime DropoffDate { get; set; }
    [Required]
    public Location PickupLocation { get; set; }
    [Required]
    public Location DropoffLocation { get; set; }
    public int RentalDays { get; set; }
    [Required]

    public string PaymentType { get; set; }
    public decimal Deposit { get; set; }
    public string DriverFullName { get; set; }
    public DateTime? DriverDob { get; set; }
    public string DriverEmail { get; set; }
    public string DriverPhoneNumber { get; set; }
    public string DriverNationalId { get; set; }
    public string DriverDrivingLicenseUri { get; set; }
    public string DriverHouseNumberStreet { get; set; }
    public Location DriverLocation { get; set; }
}