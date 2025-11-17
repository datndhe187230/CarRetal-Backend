using AutoMapper;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.Car;
using System.Linq;
using NewCar = CarRental_BE.Models.NewEntities.Car;
using NewPlan = CarRental_BE.Models.NewEntities.CarPricingPlan;

namespace CarRental_BE.Models.Mapper
{
    public class CarMapper : Profile
    {
        public CarMapper()
        {
            // New entity mappings
            CreateMap<NewCar, CarVO_ViewACar>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CarId))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => (long)(src.CarPricingPlans.Where(p => p.IsActive == true).Select(p => (decimal?)p.BasePricePerDayCents).FirstOrDefault() ?? 0m)))
                .ForMember(dest => dest.NumberOfSeats, opt => opt.MapFrom(src => (int)src.NumberOfSeats))
                .ForMember(dest => dest.ProductionYear, opt => opt.MapFrom(src => (int)src.ProductionYear))
                .ForMember(dest => dest.CarImageFront, opt => opt.MapFrom(src => src.CarImages.Where(i => i.ImageType == "front").Select(i => i.Uri).FirstOrDefault()))
                .ForMember(dest => dest.CarImageBack, opt => opt.MapFrom(src => src.CarImages.Where(i => i.ImageType == "back").Select(i => i.Uri).FirstOrDefault()))
                .ForMember(dest => dest.CarImageLeft, opt => opt.MapFrom(src => src.CarImages.Where(i => i.ImageType == "left").Select(i => i.Uri).FirstOrDefault()))
                .ForMember(dest => dest.CarImageRight, opt => opt.MapFrom(src => src.CarImages.Where(i => i.ImageType == "right").Select(i => i.Uri).FirstOrDefault()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Ward, opt => opt.MapFrom(src => src.Address != null ? src.Address.Ward : null))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.Address != null ? src.Address.District : null))
                .ForMember(dest => dest.CityProvince, opt => opt.MapFrom(src => src.Address != null ? src.Address.CityProvince : null));

            CreateMap<NewCar, CarVO_CarDetail>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CarId))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => (long)(src.CarPricingPlans.Where(p => p.IsActive == true).Select(p => (decimal?)p.BasePricePerDayCents).FirstOrDefault() ?? 0m)))
                .ForMember(dest => dest.Deposit, opt => opt.MapFrom(src => (long)(src.CarPricingPlans.Where(p => p.IsActive == true).Select(p => (decimal?)p.DepositCents).FirstOrDefault() ?? 0m)))
                .ForMember(dest => dest.NumberOfSeats, opt => opt.MapFrom(src => (int)src.NumberOfSeats))
                .ForMember(dest => dest.ProductionYear, opt => opt.MapFrom(src => (int)src.ProductionYear))
                .ForMember(dest => dest.Mileage, opt => opt.MapFrom(src => src.MileageKm.HasValue ? (double)src.MileageKm.Value : (double?)null))
                .ForMember(dest => dest.IsGasoline, opt => opt.MapFrom(src => src.FuelType.ToLower() == "gasoline"))
                .ForMember(dest => dest.IsAutomatic, opt => opt.MapFrom(src => src.Transmission.ToLower() == "automatic"))
                .ForMember(dest => dest.TermOfUse, opt => opt.MapFrom(src => src.TermOfUse))
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.LicensePlate, opt => opt.MapFrom(src => src.LicensePlate))
                .ForMember(dest => dest.HouseNumberStreet, opt => opt.MapFrom(src => src.Address != null ? src.Address.HouseNumberStreet : null))
                .ForMember(dest => dest.Ward, opt => opt.MapFrom(src => src.Address != null ? src.Address.Ward : null))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.Address != null ? src.Address.District : null))
                .ForMember(dest => dest.CityProvince, opt => opt.MapFrom(src => src.Address != null ? src.Address.CityProvince : null))
                .ForMember(dest => dest.CarImageFront, opt => opt.MapFrom(src => src.CarImages.Where(i => i.ImageType == "front").Select(i => i.Uri).FirstOrDefault()))
                .ForMember(dest => dest.CarImageBack, opt => opt.MapFrom(src => src.CarImages.Where(i => i.ImageType == "back").Select(i => i.Uri).FirstOrDefault()))
                .ForMember(dest => dest.CarImageLeft, opt => opt.MapFrom(src => src.CarImages.Where(i => i.ImageType == "left").Select(i => i.Uri).FirstOrDefault()))
                .ForMember(dest => dest.CarImageRight, opt => opt.MapFrom(src => src.CarImages.Where(i => i.ImageType == "right").Select(i => i.Uri).FirstOrDefault()))
                .ForMember(dest => dest.InsuranceUri, opt => opt.MapFrom(src => src.CarDocuments.Where(d => d.DocType == "insurance").Select(d => d.Uri).FirstOrDefault()))
                .ForMember(dest => dest.InsuranceUriIsVerified, opt => opt.MapFrom(src => src.CarDocuments.Where(d => d.DocType == "insurance").Select(d => d.Verified).FirstOrDefault()))
                .ForMember(dest => dest.RegistrationPaperUri, opt => opt.MapFrom(src => src.CarDocuments.Where(d => d.DocType == "registration").Select(d => d.Uri).FirstOrDefault()))
                .ForMember(dest => dest.RegistrationPaperUriIsVerified, opt => opt.MapFrom(src => src.CarDocuments.Where(d => d.DocType == "registration").Select(d => d.Verified).FirstOrDefault()))
                .ForMember(dest => dest.CertificateOfInspectionUri, opt => opt.MapFrom(src => src.CarDocuments.Where(d => d.DocType == "inspection").Select(d => d.Uri).FirstOrDefault()))
                .ForMember(dest => dest.CertificateOfInspectionUriIsVerified, opt => opt.MapFrom(src => src.CarDocuments.Where(d => d.DocType == "inspection").Select(d => d.Verified).FirstOrDefault()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.OwnerAccountId))
                .ForMember(dest => dest.PricePlans, opt => opt.MapFrom(src => src.CarPricingPlans))
                .ForMember(dest => dest.NumberOfRides, opt => opt.Ignore())
                .ForMember(dest => dest.Rating, opt => opt.Ignore())
                .ForMember(dest => dest.TotalRating, opt => opt.Ignore());

            CreateMap<NewCar, CarVO_Full>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CarId))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => (long)(src.CarPricingPlans.Where(p => p.IsActive == true).Select(p => (decimal?)p.BasePricePerDayCents).FirstOrDefault() ?? 0m)))
                .ForMember(dest => dest.Deposit, opt => opt.MapFrom(src => (long)(src.CarPricingPlans.Where(p => p.IsActive == true).Select(p => (decimal?)p.DepositCents).FirstOrDefault() ?? 0m)))
                .ForMember(dest => dest.NumberOfSeats, opt => opt.MapFrom(src => (int)src.NumberOfSeats))
                .ForMember(dest => dest.ProductionYear, opt => opt.MapFrom(src => (int)src.ProductionYear))
                .ForMember(dest => dest.Mileage, opt => opt.MapFrom(src => src.MileageKm.HasValue ? (double)src.MileageKm.Value : (double?)null))
                .ForMember(dest => dest.FuelConsumption, opt => opt.Ignore())
                .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src => src.FuelType.ToLower()))
                .ForMember(dest => dest.IsAutomatic, opt => opt.MapFrom(src => src.Transmission.ToLower() == "automatic"))
                .ForMember(dest => dest.TermOfUse, opt => opt.MapFrom(src => src.TermOfUse))
                .ForMember(dest => dest.AdditionalFunction, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.LicensePlate, opt => opt.MapFrom(src => src.LicensePlate))
                .ForMember(dest => dest.HouseNumberStreet, opt => opt.MapFrom(src => src.Address != null ? src.Address.HouseNumberStreet : null))
                .ForMember(dest => dest.Ward, opt => opt.MapFrom(src => src.Address != null ? src.Address.Ward : null))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.Address != null ? src.Address.District : null))
                .ForMember(dest => dest.CityProvince, opt => opt.MapFrom(src => src.Address != null ? src.Address.CityProvince : null))
                .ForMember(dest => dest.CarImageFront, opt => opt.MapFrom(src => src.CarImages.Where(i => i.ImageType == "front").Select(i => i.Uri).FirstOrDefault()))
                .ForMember(dest => dest.CarImageBack, opt => opt.MapFrom(src => src.CarImages.Where(i => i.ImageType == "back").Select(i => i.Uri).FirstOrDefault()))
                .ForMember(dest => dest.CarImageLeft, opt => opt.MapFrom(src => src.CarImages.Where(i => i.ImageType == "left").Select(i => i.Uri).FirstOrDefault()))
                .ForMember(dest => dest.CarImageRight, opt => opt.MapFrom(src => src.CarImages.Where(i => i.ImageType == "right").Select(i => i.Uri).FirstOrDefault()))
                .ForMember(dest => dest.InsuranceUri, opt => opt.MapFrom(src => src.CarDocuments.Where(d => d.DocType == "insurance").Select(d => d.Uri).FirstOrDefault()))
                .ForMember(dest => dest.InsuranceUriIsVerified, opt => opt.MapFrom(src => src.CarDocuments.Where(d => d.DocType == "insurance").Select(d => d.Verified).FirstOrDefault()))
                .ForMember(dest => dest.RegistrationPaperUri, opt => opt.MapFrom(src => src.CarDocuments.Where(d => d.DocType == "registration").Select(d => d.Uri).FirstOrDefault()))
                .ForMember(dest => dest.RegistrationPaperUriIsVerified, opt => opt.MapFrom(src => src.CarDocuments.Where(d => d.DocType == "registration").Select(d => d.Verified).FirstOrDefault()))
                .ForMember(dest => dest.CertificateOfInspectionUri, opt => opt.MapFrom(src => src.CarDocuments.Where(d => d.DocType == "inspection").Select(d => d.Uri).FirstOrDefault()))
                .ForMember(dest => dest.CertificateOfInspectionUriIsVerified, opt => opt.MapFrom(src => src.CarDocuments.Where(d => d.DocType == "inspection").Select(d => d.Verified).FirstOrDefault()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.OwnerAccountId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => (DateTime?)src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => (DateTime?)src.UpdatedAt))
                // Ignore navigation properties for VO
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<NewCar, SpecsVO>()
                .ForMember(d => d.Engine, o => o.Ignore())
                .ForMember(d => d.Fuel, o => o.MapFrom(s => s.FuelType))
                .ForMember(d => d.Transmission, o => o.MapFrom(s => s.Transmission))
                .ForMember(d => d.NumberOfSeat, o => o.MapFrom(s => s.NumberOfSeats.ToString()))
                .ForMember(d => d.ProductionYear, o => o.MapFrom(s => s.ProductionYear.ToString()))
                .ForMember(d => d.Mileage, o => o.MapFrom(s => s.MileageKm.HasValue ? s.MileageKm.Value.ToString() : string.Empty))
                .ForMember(d => d.FuelConsumption, o => o.Ignore())
                .ForMember(d => d.Color, o => o.MapFrom(s => s.Color ?? string.Empty));

            CreateMap<NewCar, CarSearchVO>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.CarId))
                .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand))
                .ForMember(d => d.Model, o => o.MapFrom(s => s.Model))
                .ForMember(d => d.Type, o => o.MapFrom(s => s.FuelType))
                .ForMember(d => d.Rating, o => o.MapFrom(s => s.AverageRating.HasValue ? (double)s.AverageRating.Value : 0d))
                .ForMember(d => d.BookedTime, o => o.Ignore())
                .ForMember(d => d.BasePrice, o => o.MapFrom(s =>
                    (long)(s.CarPricingPlans.Where(p => p.IsActive == true)
                        .Select(p => (decimal?)p.BasePricePerDayCents).FirstOrDefault() ?? 0m)))
                .ForMember(d => d.Images, o => o.MapFrom(s => s.CarImages.Select(i => i.Uri)))
                .ForMember(d => d.Specs, o => o.MapFrom(s => s))
                .ForMember(d => d.Ward, o => o.MapFrom(s => s.Address != null ? s.Address.Ward : string.Empty))
                .ForMember(d => d.District, o => o.MapFrom(s => s.Address != null ? s.Address.District : string.Empty))
                .ForMember(d => d.CityProvince, o => o.MapFrom(s => s.Address != null ? s.Address.CityProvince : string.Empty))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status));

            CreateMap<CarUpdateDTO, NewCar>()
            .ForMember(dest => dest.MileageKm, opt => opt.MapFrom(src =>
                src.Mileage.HasValue ? (decimal)src.Mileage.Value : (decimal?)null))
            .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src =>
                src.IsGasoline.HasValue && src.IsGasoline.Value ? "gasoline" : "diesel"))
            .ForMember(dest => dest.Transmission, opt => opt.MapFrom(src =>
                src.IsAutomatic.HasValue && src.IsAutomatic.Value ? "automatic" : "manual"))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            // bo qua truong 
            .ForMember(dest => dest.TermOfUse, opt => opt.Ignore())
            .ForMember(dest => dest.LicensePlate, opt => opt.Ignore())
            .ForMember(dest => dest.Brand, opt => opt.Ignore())
            .ForMember(dest => dest.Model, opt => opt.Ignore())
            .ForMember(dest => dest.Color, opt => opt.Ignore())
            .ForMember(dest => dest.ProductionYear, opt => opt.Ignore())
            .ForMember(dest => dest.NumberOfSeats, opt => opt.Ignore())
            .ForMember(dest => dest.CarId, opt => opt.Ignore())
            .ForMember(dest => dest.OwnerAccountId, opt => opt.Ignore())
            .ForMember(dest => dest.AddressId, opt => opt.Ignore())
            .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
            .ForMember(dest => dest.TotalRentals, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Address, opt => opt.Ignore())
            .ForMember(dest => dest.Bookings, opt => opt.Ignore())
            .ForMember(dest => dest.CarCalendars, opt => opt.Ignore())
            .ForMember(dest => dest.CarDocuments, opt => opt.Ignore())
            .ForMember(dest => dest.CarImages, opt => opt.Ignore())
            .ForMember(dest => dest.OwnerAccount, opt => opt.Ignore())
            .ForMember(dest => dest.Features, opt => opt.Ignore());
        }
    }
}
