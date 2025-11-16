using CarRental_BE.Data;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Enum;
using CarRental_BE.Models.VO.Car;
using CarRental_BE.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Car = CarRental_BE.Models.NewEntities.Car; // use new entity
using CarImage = CarRental_BE.Models.NewEntities.CarImage;
using CarDocument = CarRental_BE.Models.NewEntities.CarDocument;
using Address = CarRental_BE.Models.NewEntities.Address;
using Booking = CarRental_BE.Models.NewEntities.Booking;
using CarRental_BE.Models.NewEntities;

namespace CarRental_BE.Repositories.Impl
{
    public class CarRepositoryImpl : ICarRepository
    {
        private readonly CarRentalContext _context; // use new context
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

            // create address
            var address = new Address
            {
                AddressId = Guid.NewGuid(),
                HouseNumberStreet = dto.HouseNumber,
                Ward = dto.Ward,
                District = dto.District,
                CityProvince = dto.CityProvince,
                CreatedAt = DateTime.UtcNow,
            };

            var car = new Car
            {
                CarId = Guid.NewGuid(),
                LicensePlate = dto.LicensePlate,
                Brand = dto.BrandName,
                Model = dto.Model,
                Color = dto.Color,
                NumberOfSeats = byte.TryParse(dto.NumberOfSeats, out var seats) ? seats : (byte)4,
                ProductionYear = short.TryParse(dto.ProductionYear, out var year) ? year : (short)DateTime.UtcNow.Year,
                MileageKm = decimal.TryParse(dto.Mileage, out var mileage) ? mileage : null,
                FuelType = dto.Fuel.ToLower().Contains("gas") ? "gasoline" : "diesel",
                Transmission = dto.Transmission.ToLower().Contains("auto") ? "automatic" : "manual",
                Status = CarStatus.not_verified.ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OwnerAccountId = accountId,
                AddressId = address.AddressId,
                TermOfUse = BuildTermOfUse(dto)
            };

            var uploadedPublicIds = new List<string>();

            var basePriceCents = decimal.TryParse(dto.BasePrice, out var bp) ? bp : 0m;
            var depositCents = decimal.TryParse(dto.RequiredDeposit, out var dp) ? dp : 0m;

            var pricingPlan = new CarPricingPlan
            {
                PlanId = Guid.NewGuid(),
                CarId = car.CarId,
                BasePricePerDayCents = basePriceCents,
                DepositCents = depositCents,
                IsActive = true,
            };
            try
            {
                _context.Addresses.Add(address);
                _context.Cars.Add(car);
                _context.CarPricingPlans.Add(pricingPlan);

                // images
                await AddImageAsync(dto.FrontImage, car.CarId, "front", uploadedPublicIds);
                await AddImageAsync(dto.BackImage, car.CarId, "back", uploadedPublicIds);
                await AddImageAsync(dto.LeftImage, car.CarId, "left", uploadedPublicIds);
                await AddImageAsync(dto.RightImage, car.CarId, "right", uploadedPublicIds);

                // documents
                await AddDocumentAsync(dto.RegistrationPaper, car.CarId, "registration", uploadedPublicIds);
                await AddDocumentAsync(dto.CertificateOfInspection, car.CarId, "inspection", uploadedPublicIds);
                await AddDocumentAsync(dto.Insurance, car.CarId, "insurance", uploadedPublicIds);

                await _context.SaveChangesAsync();
                return car;
            }
            catch (Exception ex)
            {
                foreach (var pid in uploadedPublicIds)
                {
                    try { await _cloudinaryService.DeleteImageAsync(pid); } catch { }
                }
                throw new Exception($"Failed to save car: {ex.Message}", ex);
            }
        }

        private async Task AddImageAsync(IFormFile? file, Guid carId, string type, List<string> uploadedPublicIds)
        {
            if (file == null) return;
            var url = await _cloudinaryService.UploadImageAsync(file, "CarRental/CarImages");
            uploadedPublicIds.Add(GetPublicIdFromUrl(url));
            _context.CarImages.Add(new CarImage
            {
                ImageId = Guid.NewGuid(),
                CarId = carId,
                ImageType = type,
                Uri = url,
                IsPrimary = type == "front"
            });
        }

        private async Task AddDocumentAsync(IFormFile? file, Guid carId, string type, List<string> uploadedPublicIds)
        {
            if (file == null) return;
            var url = await _cloudinaryService.UploadImageAsync(file, "CarRental/Documents");
            uploadedPublicIds.Add(GetPublicIdFromUrl(url));
            _context.CarDocuments.Add(new CarDocument
            {
                DocId = Guid.NewGuid(),
                CarId = carId,
                DocType = type,
                Uri = url,
                Verified = false
            });
        }

        public Task<(List<Car> cars, int totalCount)> GetAccountId(Guid accountId, int pageNumber, int pageSize)
        {
            var query = _context.Cars.Where(c => c.OwnerAccountId == accountId);
            var totalCount = query.Count();
            var cars = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return Task.FromResult((cars, totalCount));
        }

        public async Task<Car?> GetByIdWithBookings(Guid carId)
        {
            return await _context.Cars
                .Include(c => c.CarPricingPlans)
                .Include(c => c.Address)
                .Include(c => c.CarImages)
                .Include(c => c.Bookings)
                .ThenInclude(b => b.Reviews)
                .FirstOrDefaultAsync(c => c.CarId == carId);
        }

        public async Task<Car?> GetByIdAsync(Guid id)
        {
            return await _context.Cars
                .Include(c => c.CarCalendars)
                .Include(c => c.CarPricingPlans)
                .FirstOrDefaultAsync(c => c.CarId == id);
        }

        private string BuildTermOfUse(AddCarDTO dto)
        {
            var terms = new List<string>();
            if (dto.NoSmoking) terms.Add("NoSmoking");
            if (dto.NoFoodInCar) terms.Add("NoFood");
            if (dto.NoPet) terms.Add("NoPet");
            if (dto.Other && !string.IsNullOrWhiteSpace(dto.OtherText)) terms.Add(dto.OtherText);
            return string.Join(',', terms);
        }

        private string GetPublicIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;
            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var uploadIndex = Array.IndexOf(segments, "upload");
                if (uploadIndex == -1 || uploadIndex + 2 >= segments.Length) return string.Empty;
                var publicIdSegments = segments.Skip(uploadIndex + 2).ToArray();
                var publicIdWithExt = string.Join('/', publicIdSegments);
                return Path.ChangeExtension(publicIdWithExt, null) ?? string.Empty;
            }
            catch { return string.Empty; }
        }

        public async Task<(List<Car> cars, int totalCount)> SearchCar(SearchDTO searchDTO, int pageNumber, int pageSize)
        {
            var query = _context.Cars.AsQueryable();

            // location via address
            query = query.Where(c =>
                (string.IsNullOrEmpty(searchDTO.LocationProvince) || (c.Address != null && c.Address.CityProvince == searchDTO.LocationProvince))
                && (string.IsNullOrEmpty(searchDTO.LocationDistrict) || (c.Address != null && c.Address.District == searchDTO.LocationDistrict))
                && (string.IsNullOrEmpty(searchDTO.LocationWard) || (c.Address != null && c.Address.Ward == searchDTO.LocationWard))
                && c.Status == "verified");

            if (searchDTO.PickupTime.HasValue && searchDTO.DropoffTime.HasValue)
            {
                var pickup = searchDTO.PickupTime.Value;
                var drop = searchDTO.DropoffTime.Value;
                query = query.Where(c => !_context.Bookings.Any(b => b.CarId == c.CarId && b.Status != "completed" && b.Status != "cancelled"
                    && ((pickup >= b.PickUpTime && pickup <= (b.ActualReturnTime ?? b.DropOffTime))
                    || (drop >= b.PickUpTime && drop <= (b.ActualReturnTime ?? b.DropOffTime))
                    || (b.PickUpTime >= pickup && b.PickUpTime <= drop))));
            }

            // active pricing plan
            query = query.Where(c => c.CarPricingPlans.Any(p => p.IsActive == true));

            if (searchDTO.CarTypes?.Any() == true)
            {
                query = query.Where(c => searchDTO.CarTypes.Contains(c.Model) || searchDTO.CarTypes.Contains(c.Brand));
            }

            if (searchDTO.FuelTypes?.Any() == true)
            {
                var fuels = searchDTO.FuelTypes.Select(f => f.ToLower()).ToArray();
                query = query.Where(c => fuels.Contains(c.FuelType.ToLower()));
            }

            if (searchDTO.TransmissionTypes?.Any() == true)
            {
                var trans = searchDTO.TransmissionTypes.Select(t => t.ToLower()).ToArray();
                query = query.Where(c => trans.Contains(c.Transmission.ToLower()));
            }

            if (searchDTO.Brands?.Any() == true)
            {
                query = query.Where(c => searchDTO.Brands.Contains(c.Brand));
            }

            if (searchDTO.Seats?.Any() == true)
            {
                var seatCounts = searchDTO.Seats.Select(s => byte.TryParse(s, out var n) ? n : (byte)0).ToArray();
                query = query.Where(c => seatCounts.Contains(c.NumberOfSeats));
            }

            if (!string.IsNullOrEmpty(searchDTO.SearchQuery))
            {
                var searchLower = searchDTO.SearchQuery.ToLower();
                query = query.Where(c => c.Brand.ToLower().Contains(searchLower) || c.Model.ToLower().Contains(searchLower));
            }

            if (!string.IsNullOrEmpty(searchDTO.SortBy))
            {
                bool asc = string.Equals(searchDTO.Order, "asc", StringComparison.OrdinalIgnoreCase);
                switch (searchDTO.SortBy.ToLower())
                {
                    case "price":
                        query = asc
                            ? query.OrderBy(c => c.CarPricingPlans.Where(p => p.IsActive == true).Select(p => p.BasePricePerDayCents).FirstOrDefault())
                            : query.OrderByDescending(c => c.CarPricingPlans.Where(p => p.IsActive == true).Select(p => p.BasePricePerDayCents).FirstOrDefault());
                        break;
                    case "newest":
                        query = asc
                            ? query.OrderBy(c => c.CreatedAt)
                            : query.OrderByDescending(c => c.CreatedAt);
                        break;
                    default:
                        query = query.OrderBy(c => c.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(c => c.CreatedAt);
            }

            var total = await query.CountAsync();
            var cars = await query
                .Include(c => c.CarPricingPlans)
                .Include(c => c.Address)
                .Include(c => c.CarImages)
                .Include(c => c.Features)
                .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (cars, total);
        }

        public async Task<List<CarSummaryDTO>> GetAllWithFeedback()
        {
            var cars = await _context.Cars
                .Include(c => c.Bookings).ThenInclude(b => b.Reviews)
                .Select(c => new CarSummaryDTO
                {
                    Id = c.CarId,
                    Brand = c.Brand,
                    Model = c.Model,
                    Color = c.Color,
                    BasePrice = (long)(_context.CarPricingPlans.Where(p => p.CarId == c.CarId && p.IsActive == true).Select(p => (decimal?)p.BasePricePerDayCents).FirstOrDefault() ?? 0m),
                    Deposit = (long)(_context.CarPricingPlans.Where(p => p.CarId == c.CarId && p.IsActive == true).Select(p => (decimal?)p.DepositCents).FirstOrDefault() ?? 0m),
                    Status = c.Status,
                    Address = c.Address != null ? c.Address.GetFullAddress() : string.Empty,
                    AverageRating = c.Bookings
                .SelectMany(b => b.Reviews)
                .Select(r => (double?)r.Rating) // rating is byte
                .DefaultIfEmpty()
                .Average()
                })
                .ToListAsync();
            return cars;
        }

        public async Task<(List<CarVO_Full> cars, int totalCount)>
    GetAllUnverifiedCarsAsync(int pageNumber, int pageSize, CarFilterDTO? filters)
        {
            var query = _context.Cars
                .Where(c => c.Status == "not_verified");

            // ------- FILTERING -------
            if (filters != null)
            {
                if (!string.IsNullOrWhiteSpace(filters.Brand))
                    query = query.Where(c => c.Brand.Contains(filters.Brand));

                if (!string.IsNullOrWhiteSpace(filters.Search))
                {
                    string keyword = filters.Search.ToLower();
                    query = query.Where(c =>
                        c.Brand.ToLower().Contains(keyword) ||
                        c.Model.ToLower().Contains(keyword));
                }

                // ------- SORTING -------
                bool asc = filters.SortDirection?.ToLower() == "asc";

                if (!string.IsNullOrWhiteSpace(filters.SortBy))
                {
                    query = filters.SortBy.ToLower() switch
                    {
                        "brand" => asc ? query.OrderBy(c => c.Brand)
                                       : query.OrderByDescending(c => c.Brand),

                        "price" => asc
                            ? query.OrderBy(c =>
                                c.CarPricingPlans.Where(p => (bool)p.IsActive)
                                .Select(p => p.BasePricePerDayCents).FirstOrDefault())
                            : query.OrderByDescending(c =>
                                c.CarPricingPlans.Where(p => (bool)p.IsActive)
                                .Select(p => p.BasePricePerDayCents).FirstOrDefault()),

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

            // ------- COUNT -------
            int totalCount = await query.CountAsync();

            // ------- PROJECTION (NO MORE NULLS!) -------
            var cars = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CarVO_Full
                {
                    Id = c.CarId,
                    Brand = c.Brand,
                    Model = c.Model,
                    Color = c.Color,
                    Deposit = (long)c.CarPricingPlans.FirstOrDefault(cp => cp.CarId == c.CarId).DepositCents,
                    NumberOfSeats = c.NumberOfSeats,
                    ProductionYear = c.ProductionYear,
                    Mileage = (double)c.MileageKm,
                    IsAutomatic = c.Transmission == "automatic",
                    FuelType = c.FuelType,
                    TermOfUse = c.TermOfUse,
                    LicensePlate = c.LicensePlate,
                    HouseNumberStreet = c.Address.HouseNumberStreet,
                    Ward = c.Address.Ward,
                    District = c.Address.District,
                    CityProvince = c.Address.CityProvince,

                    CarImageFront = c.CarImages.Where(ci => ci.ImageType == "front").FirstOrDefault(ci => ci.CarId == c.CarId).Uri,
                    CarImageBack = c.CarImages.Where(ci => ci.ImageType == "back").FirstOrDefault(ci => ci.CarId == c.CarId).Uri,
                    CarImageLeft = c.CarImages.Where(ci => ci.ImageType == "left").FirstOrDefault(ci => ci.CarId == c.CarId).Uri,
                    CarImageRight = c.CarImages.Where(ci => ci.ImageType == "right").FirstOrDefault(ci => ci.CarId == c.CarId).Uri,

                    InsuranceUri = c.CarDocuments.Where(ci => ci.DocType == "insurance").FirstOrDefault(ci => ci.CarId == c.CarId).Uri,
                    InsuranceUriIsVerified = c.CarDocuments.Where(ci => ci.DocType == "insurance").FirstOrDefault(ci => ci.CarId == c.CarId).Verified,

                    RegistrationPaperUri = c.CarDocuments.Where(ci => ci.DocType == "registration").FirstOrDefault(ci => ci.CarId == c.CarId).Uri,
                    RegistrationPaperUriIsVerified = c.CarDocuments.Where(ci => ci.DocType == "registration").FirstOrDefault(ci => ci.CarId == c.CarId).Verified,

                    CertificateOfInspectionUri = c.CarDocuments.Where(ci => ci.DocType == "inspection").FirstOrDefault(ci => ci.CarId == c.CarId).Uri,
                    CertificateOfInspectionUriIsVerified = c.CarDocuments.Where(ci => ci.DocType == "inspection").FirstOrDefault(ci => ci.CarId == c.CarId).Verified,

                    Status = c.Status,
                    AccountId = c.OwnerAccountId,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,

                    // *** PRICE ***
                    BasePrice = (long)c.CarPricingPlans
                        .Where(p => (bool)p.IsActive)
                        .Select(p => p.BasePricePerDayCents)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return (cars, totalCount);
        }

        public async Task VerifyCarInfo(Guid carId)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == carId);
            if (car == null) return;
            car.Status = CarStatus.verified.ToString();
            car.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<(List<Car> cars, int totalCount)> GetAccountCarsFilteredAsync(Guid accountId, int pageNumber, int pageSize, CarFilterDTO filters)
        {
            var query = _context.Cars.Where(c => c.OwnerAccountId == accountId);
            if (!string.IsNullOrEmpty(filters.Brand)) query = query.Where(c => c.Brand.Contains(filters.Brand));
            if (!string.IsNullOrEmpty(filters.Search))
            {
                var search = filters.Search.ToLower();
                query = query.Where(c => c.Brand.ToLower().Contains(search) || c.Model.ToLower().Contains(search));
            }
            if (!string.IsNullOrEmpty(filters.SortBy))
            {
                bool asc = string.Equals(filters.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                query = filters.SortBy.ToLower() switch
                {
                    "brand" => asc ? query.OrderBy(c => c.Brand) : query.OrderByDescending(c => c.Brand),
                    "model" => asc ? query.OrderBy(c => c.Model) : query.OrderByDescending(c => c.Model),
                    "price" => asc ? query.OrderBy(c => c.CarPricingPlans.Where(p => p.IsActive == true).Select(p => p.BasePricePerDayCents).FirstOrDefault())
                                     : query.OrderByDescending(c => c.CarPricingPlans.Where(p => p.IsActive == true).Select(p => p.BasePricePerDayCents).FirstOrDefault()),
                    _ => query.OrderByDescending(c => c.CreatedAt)
                };
            }
            else query = query.OrderByDescending(c => c.CreatedAt);

            var total = await query.CountAsync();
            var cars = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (cars, total);
        }

        public Task<bool> CheckCarBookingStatus(Guid carId, DateTime pickupDate, DateTime dropoffDate)
        {
            var hasConflict = _context.Bookings.Any(b => b.CarId == carId && b.Status != "completed" && b.Status != "cancelled" && ((pickupDate >= b.PickUpTime && pickupDate <= (b.ActualReturnTime ?? b.DropOffTime)) || (dropoffDate >= b.PickUpTime && dropoffDate <= (b.ActualReturnTime ?? b.DropOffTime)) || (b.PickUpTime >= pickupDate && b.PickUpTime <= dropoffDate)));
            return Task.FromResult(!hasConflict);
        }

        public Task<Car?> GetCarById(Guid carId)
        {
            return _context.Cars.Include(c => c.CarImages).Include(c => c.CarDocuments).FirstOrDefaultAsync(c => c.CarId == carId);
        }

        public async Task<Car?> UpdateCar(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
            return car;
        }
    }
}

