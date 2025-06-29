
using CarRental_BE.Models.VO.Transaction;

public class BookingDetailVO
{
    public string BookingNumber { get; set; }
    public string CarName { get; set; }

    public Guid CarId { get; set; }
    public string Status { get; set; }
    public DateTime? PickUpTime { get; set; }
    public DateTime? DropOffTime { get; set; }
    public string? AccountEmail { get; set; }

    // Renter's information (from Account and UserProfile)
    public string? RenterFullName { get; set; }
    public DateOnly? RenterDob { get; set; }
    public string? RenterPhoneNumber { get; set; }
    public string? RenterEmail { get; set; }
    public string? RenterNationalId { get; set; }
    public string? RenterDrivingLicenseUri { get; set; }
    public string? RenterHouseNumberStreet { get; set; }
    public string? RenterWard { get; set; }
    public string? RenterDistrict { get; set; }
    public string? RenterCityProvince { get; set; }

    // Driver's information (from Booking)
    public string? DriverFullName { get; set; }
    public DateOnly? DriverDob { get; set; }
    public string? DriverPhoneNumber { get; set; }
    public string? DriverEmail { get; set; }
    public string? DriverNationalId { get; set; }
    public string? DriverDrivingLicenseUri { get; set; }
    public string? DriverHouseNumberStreet { get; set; }
    public string? DriverWard { get; set; }
    public string? DriverDistrict { get; set; }
    public string? DriverCityProvince { get; set; }

    //check if renter is driver
    public Boolean isRenterSameAsDriver { get; set; }

    // Car information
    public string? LicensePlate { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? Color { get; set; }
    public int? ProductionYear { get; set; }
    public bool? IsAutomatic { get; set; }
    public bool? IsGasoline { get; set; }
    public int? NumberOfSeats { get; set; }
    public double? Mileage { get; set; }
    public double? FuelConsumption { get; set; }
    public string? CarAddress { get; set; }
    public string? Description { get; set; }
    public string? AdditionalFunction { get; set; }
    public string? TermOfUse { get; set; }
    public string? CarImageFront { get; set; }
    public string? CarImageBack { get; set; }
    public string? CarImageLeft { get; set; }
    public string? CarImageRight { get; set; }

    // Documents
    public string? InsuranceUri { get; set; }
    public bool? InsuranceUriIsVerified { get; set; }
    public string? RegistrationPaperUri { get; set; }
    public bool? RegistrationPaperUriIsVerified { get; set; }
    public string? CertificateOfInspectionUri { get; set; }
    public bool? CertificateOfInspectionUriIsVerified { get; set; }

    // Payment information
    public long? BasePrice { get; set; }
    public long? Deposit { get; set; }
    public string? PaymentType { get; set; }
}


