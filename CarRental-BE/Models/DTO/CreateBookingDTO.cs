using System.ComponentModel.DataAnnotations;

namespace CarRental_BE.Models.DTO
{
    public class CreateBookingDTO
    {
        [Required]
        public Guid CarId { get; }
        [Required]
        public Guid DriverId { get; }


        [Required]
        public DateTime PickupTime { get; }
        [Required]
        public DateTime DropoffTime { get; }
        [Required]
        public string PickupLocation { get; }
        [Required]
        public string DropOffLocation { get; }

        [Required]
        public int RentalDays { get; }

        public string? DriverFullName { get; }
        public DateOnly? DriverDob { get; }
        public string? DriverEmail { get; }
        public string? DriverPhoneNumber { get; }
        public string? DriverNationalId { get; }
        public string? DriverDrivingLicenseUri { get; }
        public string? DriverHouseNumberStreet { get; }
        public string? DriverWard { get; }
        public string? DriverDistrict { get; }
        public string? DriverCityProvince { get; }

        [Required]
        public string PaymentType { get; }
        [Required]
        public long Deposit { get; }
    }
}
