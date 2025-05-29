namespace CarRental_BE.Models.VO.User
{
    public class UserProfileVO
    {
        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public DateOnly? Dob { get; set; }

        public string? PhoneNumber { get; set; }

        public string? NationalId { get; set; }

        public string? DrivingLicenseUri { get; set; }

        public string? HouseNumberStreet { get; set; }

        public string? Ward { get; set; }

        public string? District { get; set; }

        public string? CityProvince { get; set; }
    }
}
