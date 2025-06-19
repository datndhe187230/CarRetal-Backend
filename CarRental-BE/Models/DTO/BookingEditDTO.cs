namespace CarRental_BE.Models.DTO
{
    public class BookingEditDTO
    {
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
        
    }
}
