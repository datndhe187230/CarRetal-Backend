using System.ComponentModel.DataAnnotations;

namespace CarRental_BE.Models.DTO
{
    public class UserUpdateDTO
    {
        [StringLength(100, ErrorMessage = "Full name must be less than 100 characters")]
        public string? FullName { get; set; }

        public DateOnly? Dob { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(20)]
        public string? NationalId { get; set; }

        [Url(ErrorMessage = "Invalid URL")]
        public string? DrivingLicenseUri { get; set; }

        [StringLength(100)]
        public string? HouseNumberStreet { get; set; }

        [StringLength(100)]
        public string? Ward { get; set; }

        [StringLength(100)]
        public string? District { get; set; }

        [StringLength(100)]
        public string? CityProvince { get; set; }
    }
}
