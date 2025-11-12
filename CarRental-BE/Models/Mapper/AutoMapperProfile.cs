using AutoMapper;
using CarRental_BE.Models.VO.Car;
using CarRental_BE.Models.VO;
using CarRental_BE.Models.DTO;
using NewCar = CarRental_BE.Models.NewEntities.Car;

namespace CarRental_BE.Models.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
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
                .ForMember(dest => dest.NumberOfRides, opt => opt.Ignore())
                .ForMember(dest => dest.Rating, opt => opt.Ignore())
                .ForMember(dest => dest.TotalRating, opt => opt.Ignore());

            CreateMap<NewCar, CarVO_Full>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CarId));
        }
    }
}
