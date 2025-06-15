using CarRental_BE.Data;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.VO.Car;
using CarRental_BE.Services;
using Microsoft.EntityFrameworkCore;

namespace CarRental_BE.Repositories.Impl
{
    public class CarRepositoryImpl : ICarRepository
    {
        private readonly CarRentalContext _context;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CarRepositoryImpl(CarRentalContext context, ICloudinaryService cloudinaryService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Car?> AddCar(AddCarDTO dto)
        {
            var idClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("id");
            var accountId = idClaim != null ? Guid.Parse(idClaim.Value) : Guid.Empty;

            var car = new Car
            {
                Id = Guid.NewGuid(),
                LicensePlate = dto.LicensePlate,
                Brand = dto.BrandName,
                Model = dto.Model,
                Color = dto.Color,
                NumberOfSeats = int.TryParse(dto.NumberOfSeats, out var seats) ? seats : null,
                ProductionYear = int.TryParse(dto.ProductionYear, out var year) ? year : null,
                Mileage = double.TryParse(dto.Mileage, out var mileage) ? mileage : null,
                FuelConsumption = double.TryParse(dto.FuelConsumption, out var fc) ? fc : null,
                BasePrice = long.TryParse(dto.BasePrice, out var price) ? price : 0,
                Deposit = long.TryParse(dto.RequiredDeposit, out var deposit) ? deposit : 0,

                IsGasoline = dto.Fuel.ToLower().Contains("gas"), // crude check
                IsAutomatic = dto.Transmission.ToLower().Contains("auto"),

                Description = dto.Description,
                TermOfUse = BuildTermOfUse(dto),
                AdditionalFunction = BuildAdditionalFunctions(dto),

                HouseNumberStreet = dto.HouseNumber,
                Ward = dto.Ward,
                District = dto.District,
                CityProvince = dto.CityProvince,

                Status = "Available",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AccountId = accountId
    };

            // Upload Images
            if (dto.FrontImage != null)
                car.CarImageFront = await _cloudinaryService.UploadImageAsync(dto.FrontImage, "CarImages");
            if (dto.BackImage != null)
                car.CarImageBack = await _cloudinaryService.UploadImageAsync(dto.BackImage, "CarImages");
            if (dto.LeftImage != null)
                car.CarImageLeft = await _cloudinaryService.UploadImageAsync(dto.LeftImage, "CarImages");
            if (dto.RightImage != null)
                car.CarImageRight = await _cloudinaryService.UploadImageAsync(dto.RightImage, "CarImages");

            // Upload Documents
            if (dto.RegistrationPaper != null)
            {
                car.RegistrationPaperUri = await _cloudinaryService.UploadImageAsync(dto.RegistrationPaper, "Documents");
                car.RegistrationPaperUriIsVerified = false;
            }
            if (dto.CertificateOfInspection != null)
            {
                car.CertificateOfInspectionUri = await _cloudinaryService.UploadImageAsync(dto.CertificateOfInspection, "Documents");
                car.CertificateOfInspectionUriIsVerified = false;
            }
            if (dto.Insurance != null)
            {
                car.InsuranceUri = await _cloudinaryService.UploadImageAsync(dto.Insurance, "Documents");
                car.InsuranceUriIsVerified = false;
            }

            // Save to DB
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return car;
        }

        public Task<(List<Car> cars, int totalCount)> GetAccountId(
            Guid accountId, 
            int pageNumber, 
            int pageSize)
        {
            var query = _context.Cars.Where(c => c.AccountId == accountId);

            var totalCount = query.Count();
            var cars = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        return Task.FromResult((cars: cars.Result, totalCount: totalCount));

        }

        public async Task<Car?> GetByIdWithBookings(Guid carId)
        {
            return await _context.Cars
                .Include(c => c.Bookings)
                    .ThenInclude(b => b.Feedback)  // Add this line
                .FirstOrDefaultAsync(c => c.Id == carId);
        }

        private string BuildAdditionalFunctions(AddCarDTO dto)
        {
            var functions = new List<string>();
            if (dto.Bluetooth) functions.Add("Bluetooth");
            if (dto.GPS) functions.Add("GPS");
            if (dto.Camera) functions.Add("Camera");
            if (dto.SunRoof) functions.Add("SunRoof");
            if (dto.ChildLock) functions.Add("ChildLock");
            if (dto.ChildSeat) functions.Add("ChildSeat");
            if (dto.DVD) functions.Add("DVD");
            if (dto.USB) functions.Add("USB");
            return string.Join(",", functions);
        }

        private string BuildTermOfUse(AddCarDTO dto)
        {
            var terms = new List<string>();
            if (dto.NoSmoking) terms.Add("NoSmoking");
            if (dto.NoFoodInCar) terms.Add("NoFood");
            if (dto.NoPet) terms.Add("NoPet");
            if (dto.Other && !string.IsNullOrWhiteSpace(dto.OtherText))
                terms.Add(dto.OtherText);
            return string.Join(",", terms);
        }

    }
}
