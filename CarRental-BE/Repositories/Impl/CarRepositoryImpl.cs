using CarRental_BE.Data;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.Enum;
using CarRental_BE.Models.VO.Car;
using CarRental_BE.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

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

                Status = CarStatus.not_verified.ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AccountId = accountId
    };

            var uploadedImagePublicIds = new List<string>();

            try
            {
                // Upload Images
                if (dto.FrontImage != null)
                {
                    var frontUrl = await _cloudinaryService.UploadImageAsync(dto.FrontImage, "CarRental/CarImages");
                    car.CarImageFront = frontUrl;
                    uploadedImagePublicIds.Add(GetPublicIdFromUrl(frontUrl));
                }

                if (dto.BackImage != null)
                {
                    var backUrl = await _cloudinaryService.UploadImageAsync(dto.BackImage, "CarRental/CarImages");
                    car.CarImageBack = backUrl;
                    uploadedImagePublicIds.Add(GetPublicIdFromUrl(backUrl));
                }

                if (dto.LeftImage != null)
                {
                    var leftUrl = await _cloudinaryService.UploadImageAsync(dto.LeftImage, "CarRental/CarImages");
                    car.CarImageLeft = leftUrl;
                    uploadedImagePublicIds.Add(GetPublicIdFromUrl(leftUrl));
                }

                if (dto.RightImage != null)
                {
                    var rightUrl = await _cloudinaryService.UploadImageAsync(dto.RightImage, "CarRental/CarImages");
                    car.CarImageRight = rightUrl;
                    uploadedImagePublicIds.Add(GetPublicIdFromUrl(rightUrl));
                }

                // Documents
                if (dto.RegistrationPaper != null)
                {
                    var docUrl = await _cloudinaryService.UploadImageAsync(dto.RegistrationPaper, "CarRental/Documents");
                    car.RegistrationPaperUri = docUrl;
                    car.RegistrationPaperUriIsVerified = false;
                    uploadedImagePublicIds.Add(GetPublicIdFromUrl(docUrl));
                }

                if (dto.CertificateOfInspection != null)
                {
                    var certUrl = await _cloudinaryService.UploadImageAsync(dto.CertificateOfInspection, "CarRental/Documents");
                    car.CertificateOfInspectionUri = certUrl;
                    car.CertificateOfInspectionUriIsVerified = false;
                    uploadedImagePublicIds.Add(GetPublicIdFromUrl(certUrl));
                }

                if (dto.Insurance != null)
                {
                    var insUrl = await _cloudinaryService.UploadImageAsync(dto.Insurance, "CarRental/Documents");
                    car.InsuranceUri = insUrl;
                    car.InsuranceUriIsVerified = false;
                    uploadedImagePublicIds.Add(GetPublicIdFromUrl(insUrl));
                }

                // Save to DB
                _context.Cars.Add(car);
                await _context.SaveChangesAsync();

                return car;
            }
            catch (Exception ex)
            {
                // Rollback Cloudinary uploads
                foreach (var publicId in uploadedImagePublicIds)
                {
                    try
                    {
                        await _cloudinaryService.DeleteImageAsync(publicId);
                    }
                    catch (Exception deleteEx)
                    {
                        // Log deleteEx or ignore
                    }
                }

                throw new Exception($"Failed to save car: {ex.Message}", ex);
            }
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
        public async Task<Car?> GetByIdAsync(Guid id)
        {
            return await _context.Cars.FirstOrDefaultAsync(c => c.Id == id);
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

        private string  GetPublicIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                // Find the index of the "upload" segment
                var uploadIndex = Array.IndexOf(segments, "upload");
                if (uploadIndex == -1 || uploadIndex + 2 >= segments.Length)
                    return string.Empty;

                // Everything after upload and version is the public ID (joined if in folders)
                var publicIdSegments = segments.Skip(uploadIndex + 2).ToArray(); // skip "v123456789"
                var publicIdWithExt = string.Join("/", publicIdSegments); // supports folders
                var publicId = Path.ChangeExtension(publicIdWithExt, null); // remove file extension

                return publicId;
            }
            catch
            {
                return string.Empty;
            }
        }


        public async Task<(List<Car> cars, int totalCount)> SearchCar(SearchDTO searchDTO, int pageNumber, int pageSize)
        {
            var query = _context.Cars.AsQueryable();

            // Apply location filters
            query = query.Where(c => (string.IsNullOrEmpty(searchDTO.LocationProvince) || c.CityProvince == searchDTO.LocationProvince)
                                  && (string.IsNullOrEmpty(searchDTO.LocationDistrict) || c.District == searchDTO.LocationDistrict)
                                  && (string.IsNullOrEmpty(searchDTO.LocationWard) || c.Ward == searchDTO.LocationWard)
                                  && c.Status == "verified");

            // Filter out cars with conflicting bookings
            if (searchDTO.PickupTime.HasValue && searchDTO.DropoffTime.HasValue)
            {
                query = query.Where(c => !_context.Bookings.Any(b => b.CarId == c.Id
                                                                && b.Status != "completed" && b.Status != "cancelled"
                                                                && (searchDTO.PickupTime >= b.PickUpTime && searchDTO.PickupTime <= b.DropOffTime
                                                                    || searchDTO.DropoffTime >= b.PickUpTime && searchDTO.DropoffTime <= b.DropOffTime
                                                                    || b.PickUpTime >= searchDTO.PickupTime && b.PickUpTime <= searchDTO.DropoffTime)));
            }

            // Apply price range filter
            query = query.Where(c => c.BasePrice >= searchDTO.PriceRangeMin && c.BasePrice <= searchDTO.PriceRangeMax);

            // Apply car type filter
            if (searchDTO.CarTypes != null && searchDTO.CarTypes.Any())
            {
                query = query.Where(c => searchDTO.CarTypes.Contains(c.Model) || searchDTO.CarTypes.Contains(c.Brand));
            }

            // Apply fuel type filter
            if (searchDTO.FuelTypes != null && searchDTO.FuelTypes.Any())
            {
                bool[] fuelBools = searchDTO.FuelTypes.Select(f => f.ToLower() == "gasoline").ToArray();
                query = query.Where(c => fuelBools.Contains(c.IsGasoline ?? false));
            }

            // Apply transmission type filter
            if (searchDTO.TransmissionTypes != null && searchDTO.TransmissionTypes.Any())
            {
                bool[] transmissionBools = searchDTO.TransmissionTypes.Select(t => t.ToLower() == "automatic").ToArray();
                query = query.Where(c => transmissionBools.Contains(c.IsAutomatic ?? false));
            }

            // Apply brand filter
            if (searchDTO.Brands != null && searchDTO.Brands.Any())
            {
                query = query.Where(c => searchDTO.Brands.Contains(c.Brand));
            }

            // Apply seats filter
            if (searchDTO.Seats != null && searchDTO.Seats.Any())
            {
                int[] seatCounts = searchDTO.Seats.Select(s => int.TryParse(s, out int n) ? n : 0).ToArray();
                query = query.Where(c => c.NumberOfSeats.HasValue && seatCounts.Contains(c.NumberOfSeats.Value));
            }

            // Apply search query filter
            if (!string.IsNullOrEmpty(searchDTO.SearchQuery))
            {
                string searchLower = searchDTO.SearchQuery.ToLower();
                query = query.Where(c => (c.Brand != null && c.Brand.ToLower().Contains(searchLower))
                                      || (c.Model != null && c.Model.ToLower().Contains(searchLower))
                                      || (c.Description != null && c.Description.ToLower().Contains(searchLower)));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(searchDTO.SortBy))
            {
                bool isAscending = string.Equals(searchDTO.Order, "asc", StringComparison.OrdinalIgnoreCase);

                switch (searchDTO.SortBy.ToLower())
                {
                    case "price":
                        query = isAscending
                            ? query.OrderBy(c => c.BasePrice)
                            : query.OrderByDescending(c => c.BasePrice);
                        break;
                    case "newest":
                        query = isAscending
                            ? query.OrderBy(c => c.CreatedAt)
                            : query.OrderByDescending(c => c.CreatedAt);
                        break;
                    default:
                        query = query.OrderBy(c => c.BasePrice); // Default to price ascending
                        break;
                }
            }
            else
            {
                query = query.OrderBy(c => c.BasePrice); // Default sorting
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var cars = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (cars: cars, totalCount: totalCount);
        }
        public async Task<List<CarSummaryDTO>> GetAllWithFeedback()
        {
            var cars = await _context.Cars
                .Include(c => c.Bookings)
                    .ThenInclude(b => b.Feedback)
                .Select(c => new CarSummaryDTO
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Color = c.Color,
                    BasePrice = c.BasePrice,
                    Deposit = c.Deposit,
                    Status = c.Status,
                    Address = c.HouseNumberStreet + ", " + c.Ward + ", " + c.District + ", " + c.CityProvince,
                    AverageRating = c.Bookings
                        .Where(b => b.Feedback != null && b.Feedback.Rating.HasValue)
                        .Select(b => (double?)b.Feedback.Rating.Value)
                        .DefaultIfEmpty()
                        .Average()
                })
                .ToListAsync();

            return cars;
        }



        public async Task<(List<Car> cars, int totalCount)> GetAllUnverifiedCarsAsync(
    int pageNumber,
    int pageSize,
    CarFilterDTO? filters = null)
        {
            var query = _context.Cars
                .Where(c => c.Status == "not_verified");

            // Áp dụng bộ lọc nếu có
            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.Brand))
                {
                    query = query.Where(c => c.Brand.Contains(filters.Brand));
                }

                if (!string.IsNullOrEmpty(filters.Search))
                {
                    var search = filters.Search.ToLower();
                    query = query.Where(c =>
                        (!string.IsNullOrEmpty(c.Brand) && c.Brand.ToLower().Contains(search)) ||
                        (!string.IsNullOrEmpty(c.Model) && c.Model.ToLower().Contains(search)) ||
                        (!string.IsNullOrEmpty(c.Description) && c.Description.ToLower().Contains(search))
                    );
                }

                // Sắp xếp
                if (!string.IsNullOrEmpty(filters.SortBy))
                {
                    bool ascending = string.Equals(filters.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);

                    query = filters.SortBy.ToLower() switch
                    {
                        "brand" => ascending ? query.OrderBy(c => c.Brand) : query.OrderByDescending(c => c.Brand),
                        "model" => ascending ? query.OrderBy(c => c.Model) : query.OrderByDescending(c => c.Model),
                        "price" => ascending ? query.OrderBy(c => c.BasePrice) : query.OrderByDescending(c => c.BasePrice),
                        _ => query.OrderByDescending(c => c.CreatedAt)
                    };
                }
                else
                {
                    query = query.OrderByDescending(c => c.CreatedAt);
                }
            }
            else
            {
                query = query.OrderByDescending(c => c.CreatedAt);
            }

            var totalCount = await query.CountAsync();
            var cars = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (cars, totalCount);
        }

        public async Task VerifyCarInfo(Guid carId)
        {
            _context.Cars
                .Where(c => c.Id == carId)
                .ExecuteUpdate(c => c.SetProperty(car => car.Status, CarStatus.verified.ToString())
                .SetProperty(car => car.CertificateOfInspectionUriIsVerified, true)
                .SetProperty(car => car.RegistrationPaperUriIsVerified, true)
                .SetProperty(car => car.InsuranceUriIsVerified, true));

            await _context.SaveChangesAsync();
        }
        public async Task<(List<Car> cars, int totalCount)> GetAccountCarsFilteredAsync(Guid accountId, int pageNumber, int pageSize, CarFilterDTO filters)
        {
            var query = _context.Cars
                .Where(c => c.AccountId == accountId);

            if (!string.IsNullOrEmpty(filters.Brand))
            {
                query = query.Where(c => c.Brand.Contains(filters.Brand));
            }

            if (!string.IsNullOrEmpty(filters.Search))
            {
                var search = filters.Search.ToLower();
                query = query.Where(c =>
                    (!string.IsNullOrEmpty(c.Brand) && c.Brand.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(c.Model) && c.Model.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(c.Description) && c.Description.ToLower().Contains(search))
                );
            }

            // Sorting
            if (!string.IsNullOrEmpty(filters.SortBy))
            {
                bool ascending = string.Equals(filters.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);

                query = filters.SortBy.ToLower() switch
                {
                    "brand" => ascending ? query.OrderBy(c => c.Brand) : query.OrderByDescending(c => c.Brand),
                    "model" => ascending ? query.OrderBy(c => c.Model) : query.OrderByDescending(c => c.Model),
                    "price" => ascending ? query.OrderBy(c => c.BasePrice) : query.OrderByDescending(c => c.BasePrice),
                    _ => query.OrderByDescending(c => c.CreatedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(c => c.CreatedAt);
            }

            var totalCount = await query.CountAsync();
            var cars = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (cars, totalCount);
        }

        public Task<bool> CheckCarBookingStatus(Guid carId, DateTime pickupDate, DateTime dropoffDate)
        {
            throw new NotImplementedException();
        }
    }
}
