using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Enum;
using CarRental_BE.Models.VO;
using CarRental_BE.Models.VO.Booking;
using CarRental_BE.Models.VO.Transaction;
using NewBooking = CarRental_BE.Models.NewEntities.Booking;

public static class BookingMapper
{
    public static BookingVO ToBookingVO(NewBooking booking)
    {
        var numberOfDays = (booking.DropOffTime - booking.PickUpTime).Days;
        var totalPrice = numberOfDays * (long)(booking.BasePriceSnapshotCents ??0);
        return new BookingVO
        {
            BookingNumber = booking.BookingNumber,
            CarName = $"{booking.Car?.Brand} {booking.Car?.Model}" ?? "Unknown",
            PickupDate = booking.PickUpTime,
            ReturnDate = booking.DropOffTime,
            PickUpLocation = booking.PickUpAddress?.CityProvince,
            DropOffLocation = booking.DropOffAddress?.CityProvince,
            BasePrice = (long?)booking.BasePriceSnapshotCents,
            Deposit = (long?)booking.DepositSnapshotCents,
            TotalPrice = totalPrice,
            NumberOfDays = numberOfDays,
            PaymentType = booking.PaymentMethod,
            Status = booking.Status,
            CreatedAt = booking.CreatedAt,
            CarImageFront = booking.Car?.CarImages.Where(i => i.ImageType == "front").Select(i => i.Uri).FirstOrDefault(),
            CarImageBack = booking.Car?.CarImages.Where(i => i.ImageType == "back").Select(i => i.Uri).FirstOrDefault(),
            CarImageLeft = booking.Car?.CarImages.Where(i => i.ImageType == "left").Select(i => i.Uri).FirstOrDefault(),
            CarImageRight = booking.Car?.CarImages.Where(i => i.ImageType == "right").Select(i => i.Uri).FirstOrDefault()
        };
    }

    public static BookingDetailVO ToBookingDetailVO(NewBooking booking)
    {
        var car = booking.Car;
        var account = booking.RenterAccount;
        var userProfile = account?.UserProfile;
        var transactions = booking.Transactions?.Select(t => new TransactionVO
        {
            Amount = (long)t.AmountCents,
            Message = t.Description,
            CreatedAt = t.CreatedAt,
            Status = t.Status,
            Type = t.Type
        }).ToList();

        var history = booking.BookingStatusHistories?.OrderBy(h => h.ChangedAt)
            .Select(h => new BookingStatusHistoryVO
            {
                OldStatus = h.OldStatus,
                NewStatus = h.NewStatus,
                Note = h.Note,
                PictureUrl = h.PictureUrl,
                ChangedAt = h.ChangedAt
            }).ToList();

        return new BookingDetailVO
        {
            BookingNumber = booking.BookingNumber,
            CarName = $"{car?.Brand} {car?.Model}",
            CarId = car.CarId,
            Status = booking.Status,
            PickUpTime = booking.PickUpTime,
            ActualReturnTime = booking.ActualReturnTime,
            DropOffTime = booking.DropOffTime,
            AccountEmail = account?.Email,
            PickUpLocation = booking.PickUpAddress?.GetFullAddress(),
            DropOffLocation = booking.DropOffAddress?.GetFullAddress(),
            RenterFullName = userProfile?.FullName,
            RenterDob = userProfile?.Dob.HasValue == true ? userProfile.Dob : null,
            RenterPhoneNumber = userProfile?.PhoneNumber,
            RenterEmail = account?.Email,
            RenterNationalId = userProfile?.NationalId,
            RenterDrivingLicenseUri = userProfile?.DrivingLicenseUri,
            RenterHouseNumberStreet = userProfile?.Address?.HouseNumberStreet,
            RenterWard = userProfile?.Address?.Ward,
            RenterDistrict = userProfile?.Address?.District,
            RenterCityProvince = userProfile?.Address?.CityProvince,

            // Driver's information (first driver)
            DriverFullName = booking.BookingDrivers.FirstOrDefault() != null ? booking.BookingDrivers.First().FullName : null,
            DriverDob = booking.BookingDrivers.FirstOrDefault() != null ? (DateOnly?)booking.BookingDrivers.First().Dob : null,
            DriverPhoneNumber = booking.BookingDrivers.FirstOrDefault() != null ? booking.BookingDrivers.First().PhoneNumber : null,
            DriverEmail = null,
            DriverNationalId = booking.BookingDrivers.FirstOrDefault() != null ? booking.BookingDrivers.First().NationalId : null,
            DriverDrivingLicenseUri = booking.BookingDrivers.FirstOrDefault() != null ? booking.BookingDrivers.First().DrivingLicenseUri : null,
            DriverHouseNumberStreet = booking.BookingDrivers.FirstOrDefault() != null ? booking.BookingDrivers.First().Address?.HouseNumberStreet : null,
            DriverWard = booking.BookingDrivers.FirstOrDefault() != null ? booking.BookingDrivers.First().Address?.Ward : null,
            DriverDistrict = booking.BookingDrivers.FirstOrDefault() != null ? booking.BookingDrivers.First().Address?.District : null,
            DriverCityProvince = booking.BookingDrivers.FirstOrDefault() != null ? booking.BookingDrivers.First().Address?.CityProvince : null,

            isRenterSameAsDriver = false,

            // Car information
            LicensePlate = car?.LicensePlate,
            Brand = car?.Brand,
            Model = car?.Model,
            Color = car?.Color,
            ProductionYear = car != null ? (int?)car.ProductionYear : null,
            IsAutomatic = car?.Transmission.ToLower() == "automatic",
            IsGasoline = car?.FuelType.ToLower() == "gasoline",
            NumberOfSeats = car != null ? (int?)car.NumberOfSeats : null,
            Mileage = car?.MileageKm.HasValue == true ? (double?)car.MileageKm.Value : null,
            FuelConsumption = null,
            CarAddress = car?.Address != null ? $"{car.Address.HouseNumberStreet}, {car.Address.Ward}, {car.Address.District}, {car.Address.CityProvince}" : null,
            Description = null,
            AdditionalFunction = null,
            TermOfUse = car?.TermOfUse,
            CarImageFront = car?.CarImages.Where(i => i.ImageType == "front").Select(i => i.Uri).FirstOrDefault(),
            CarImageBack = car?.CarImages.Where(i => i.ImageType == "back").Select(i => i.Uri).FirstOrDefault(),
            CarImageLeft = car?.CarImages.Where(i => i.ImageType == "left").Select(i => i.Uri).FirstOrDefault(),
            CarImageRight = car?.CarImages.Where(i => i.ImageType == "right").Select(i => i.Uri).FirstOrDefault(),

            // Documents
            InsuranceUri = car?.CarDocuments.Where(d => d.DocType == "insurance").Select(d => d.Uri).FirstOrDefault(),
            InsuranceUriIsVerified = car?.CarDocuments.Where(d => d.DocType == "insurance").Select(d => d.Verified).FirstOrDefault(),
            RegistrationPaperUri = car?.CarDocuments.Where(d => d.DocType == "registration").Select(d => d.Uri).FirstOrDefault(),
            RegistrationPaperUriIsVerified = car?.CarDocuments.Where(d => d.DocType == "registration").Select(d => d.Verified).FirstOrDefault(),
            CertificateOfInspectionUri = car?.CarDocuments.Where(d => d.DocType == "inspection").Select(d => d.Uri).FirstOrDefault(),
            CertificateOfInspectionUriIsVerified = car?.CarDocuments.Where(d => d.DocType == "inspection").Select(d => d.Verified).FirstOrDefault(),

            // Payment information
            BasePrice = (long?)booking.BasePriceSnapshotCents,
            Deposit = (long?)booking.DepositSnapshotCents,
            PaymentType = booking.PaymentMethod,

            // Status history
            statusHistory = history
        };
    }

    internal static NewBooking ToBookingEntity(BookingCreateDTO bookingCreateDto, Guid userId, BookingStatusEnum status, decimal basedPrice, string bookingNumber)
    {
        return new NewBooking
        {
            BookingNumber = bookingNumber,
            CarId = bookingCreateDto.CarId,
            RenterAccountId = userId,
            PickUpTime = bookingCreateDto.PickupDate,
            DropOffTime = bookingCreateDto.DropoffDate,
            PickUpAddressId = null,
            DropOffAddressId = null,
            BasePriceSnapshotCents = basedPrice,
            DepositSnapshotCents = bookingCreateDto.Deposit,
            PaymentMethod = bookingCreateDto.PaymentType,
            Status = status.ToString(),
            CreatedAt = DateTime.UtcNow,
        };
    }

    internal static BookingInformationVO? ToBookingInformationVO(NewBooking newBooking)
    {
        throw new NotImplementedException();
    }
}
