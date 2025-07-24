using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.Enum;
using CarRental_BE.Models.VO;
using CarRental_BE.Models.VO.Booking;
using CarRental_BE.Models.VO.Transaction;

public static class BookingMapper
{

    public static BookingVO ToBookingVO(Booking booking)
    {
        var numberOfDays = (booking.DropOffTime - booking.PickUpTime)?.Days ?? 0;
        var totalPrice = numberOfDays * (booking.BasePrice ?? 0);

        return new BookingVO
        {
            BookingNumber = booking.BookingNumber,
            CarName = $"{booking.Car?.Brand} {booking.Car?.Model}" ?? "Unknown",
            PickupDate = booking.PickUpTime,
            ReturnDate = booking.DropOffTime,
            PickUpLocation = booking.PickUpLocation,
            DropOffLocation = booking.DropOffLocation,
            BasePrice = booking.BasePrice,
            Deposit = booking.Deposit,
            TotalPrice = totalPrice,
            NumberOfDays = numberOfDays,
            PaymentType = booking.PaymentType,
            Status = booking.Status,
            CreatedAt = booking.CreatedAt,
            CarImageUrl = booking.Car?.CarImageFront        // hoặc chọn ảnh khác
        };
    }

    public static BookingDetailVO ToBookingDetailVO(Booking booking)
    {
        var car = booking.Car;
        var account = booking.Account;
        var userProfile = account?.UserProfile;
        var transactions = booking.Transactions?.Select(t => new TransactionVO
        {
            Amount = t.Amount,
            Message = t.Message,
            CreatedAt = t.CreatedAt,
            Status = t.Status,
            Type = t.Type
        }).ToList();

        return new BookingDetailVO
        {
            BookingNumber = booking.BookingNumber,
            CarName = $"{car?.Brand} {car?.Model}",
            CarId = car.Id,
            Status = booking.Status,
            PickUpTime = booking.PickUpTime,
            DropOffTime = booking.DropOffTime,
            AccountEmail = account?.Email,

            // Renter's information
            RenterFullName = userProfile?.FullName,
            RenterDob = userProfile?.Dob,
            RenterPhoneNumber = userProfile?.PhoneNumber,
            RenterEmail = account?.Email,
            RenterNationalId = userProfile?.NationalId,
            RenterDrivingLicenseUri = userProfile?.DrivingLicenseUri,
            RenterHouseNumberStreet = userProfile?.HouseNumberStreet,
            RenterWard = userProfile?.Ward,
            RenterDistrict = userProfile?.District,
            RenterCityProvince = userProfile?.CityProvince,

            // Driver's information
            DriverFullName = booking.DriverFullName,
            DriverDob = booking.DriverDob,
            DriverPhoneNumber = booking.DriverPhoneNumber,
            DriverEmail = booking.DriverEmail,
            DriverNationalId = booking.DriverNationalId,
            DriverDrivingLicenseUri = booking.DriverDrivingLicenseUri,
            DriverHouseNumberStreet = booking.DriverHouseNumberStreet,
            DriverWard = booking.DriverWard,
            DriverDistrict = booking.DriverDistrict,
            DriverCityProvince = booking.DriverCityProvince,

            //check renter is driver
            isRenterSameAsDriver =
                        booking.DriverFullName.Trim() == userProfile?.FullName.Trim() &&
                        booking.DriverDob == userProfile?.Dob &&
                        booking.DriverPhoneNumber == userProfile?.PhoneNumber &&
                        booking.DriverEmail == account?.Email &&
                        booking.DriverNationalId == userProfile?.NationalId &&
                        //booking.DriverDrivingLicenseUri == userProfile?.DrivingLicenseUri &&
                        booking.DriverHouseNumberStreet == userProfile?.HouseNumberStreet &&
                        booking.DriverWard == userProfile?.Ward &&
                        booking.DriverDistrict == userProfile?.District &&
                        booking.DriverCityProvince == userProfile?.CityProvince
                        ? true : false,

            // Car information
            LicensePlate = car?.LicensePlate,
            Brand = car?.Brand,
            Model = car?.Model,
            Color = car?.Color,
            ProductionYear = car?.ProductionYear,
            IsAutomatic = car?.IsAutomatic,
            IsGasoline = car?.IsGasoline,
            NumberOfSeats = car?.NumberOfSeats,
            Mileage = car?.Mileage,
            FuelConsumption = car?.FuelConsumption,
            CarAddress = $"{car?.HouseNumberStreet}, {car?.Ward}, {car?.District}, {car?.CityProvince}",
            Description = car?.Description,
            AdditionalFunction = car?.AdditionalFunction,
            TermOfUse = car?.TermOfUse,
            CarImageFront = car?.CarImageFront,
            CarImageBack = car?.CarImageBack,
            CarImageLeft = car?.CarImageLeft,
            CarImageRight = car?.CarImageRight,

            // Documents
            InsuranceUri = car?.InsuranceUri,
            InsuranceUriIsVerified = car?.InsuranceUriIsVerified,
            RegistrationPaperUri = car?.RegistrationPaperUri,
            RegistrationPaperUriIsVerified = car?.RegistrationPaperUriIsVerified,
            CertificateOfInspectionUri = car?.CertificateOfInspectionUri,
            CertificateOfInspectionUriIsVerified = car?.CertificateOfInspectionUriIsVerified,

            // Payment information
            BasePrice = booking.BasePrice,
            Deposit = booking.Deposit,
            PaymentType = booking.PaymentType,
        };
    }

    internal static Booking ToBookingEntity(BookingCreateDTO bookingCreateDto, Guid userId, BookingStatusEnum status, decimal basedPrice, string bookingNumber)
    {
        return new Booking
        {
            BookingNumber = bookingNumber,
            CarId = bookingCreateDto.CarId,
            AccountId = userId,
            PickUpTime = bookingCreateDto.PickupDate,
            DropOffTime = bookingCreateDto.DropoffDate,
            PickUpLocation = $"{bookingCreateDto.PickupLocation.Province}/{bookingCreateDto.PickupLocation.District}/{bookingCreateDto.PickupLocation.Ward}",
            DropOffLocation = $"{bookingCreateDto.PickupLocation.Province}/{bookingCreateDto.PickupLocation.District}/{bookingCreateDto.PickupLocation.Ward}",
            BasePrice = (long?)basedPrice,
            Deposit = (long?)bookingCreateDto.Deposit,
            PaymentType = bookingCreateDto.PaymentType,
            Status = status.ToString(),
            CreatedAt = DateTime.UtcNow,
            DriverFullName = bookingCreateDto.DriverFullName,
            DriverEmail = bookingCreateDto.DriverEmail,
            DriverDob = bookingCreateDto.DriverDob == null ? null : DateOnly.FromDateTime((DateTime)bookingCreateDto.DriverDob),
            DriverPhoneNumber = bookingCreateDto.DriverPhoneNumber,
            DriverNationalId = bookingCreateDto.DriverNationalId,
            DriverHouseNumberStreet = bookingCreateDto.DriverHouseNumberStreet,
            DriverWard = bookingCreateDto.DriverLocation?.Ward,
            DriverDistrict = bookingCreateDto.DriverLocation?.District,
            DriverCityProvince = bookingCreateDto.DriverLocation?.Province
        };
    }

    internal static BookingInformationVO? ToBookingInformationVO(Booking newBooking)
    {
        throw new NotImplementedException();
    }
}
