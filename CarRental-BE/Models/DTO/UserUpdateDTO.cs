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

        public string? DrivingLicenseUri { get; set; }

         
        public string? HouseNumberStreet { get; set; }

        public string? Ward { get; set; }

      
        public string? District { get; set; }

      
        public string? CityProvince { get; set; }

        public string? Email { get; set; }
    }
}
