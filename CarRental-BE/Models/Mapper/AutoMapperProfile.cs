using AutoMapper;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.VO.Car;
using CarRental_BE.Models.VO;
using CarRental_BE.Models.DTO;

namespace CarRental_BE.Models.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Car, CarVO_ViewACar>()

                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => src.BasePrice))
                .ForMember(dest => dest.NumberOfSeats, opt => opt.MapFrom(src => src.NumberOfSeats))
                .ForMember(dest => dest.ProductionYear, opt => opt.MapFrom(src => src.ProductionYear))
                .ForMember(dest => dest.CarImageFront, opt => opt.MapFrom(src => src.CarImageFront))
                .ForMember(dest => dest.CarImageBack, opt => opt.MapFrom(src => src.CarImageBack))
                .ForMember(dest => dest.CarImageLeft, opt => opt.MapFrom(src => src.CarImageLeft))
                .ForMember(dest => dest.CarImageRight, opt => opt.MapFrom(src => src.CarImageRight))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Ward, opt => opt.MapFrom(src => src.Ward))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District))
                .ForMember(dest => dest.CityProvince, opt => opt.MapFrom(src => src.CityProvince));

            // Add new mapping for CarVO_CarDetail
            CreateMap<Car, CarVO_CarDetail>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => src.BasePrice))
                .ForMember(dest => dest.Deposit, opt => opt.MapFrom(src => src.Deposit))
                .ForMember(dest => dest.NumberOfSeats, opt => opt.MapFrom(src => src.NumberOfSeats))
                .ForMember(dest => dest.ProductionYear, opt => opt.MapFrom(src => src.ProductionYear))
                .ForMember(dest => dest.Mileage, opt => opt.MapFrom(src => src.Mileage))
                .ForMember(dest => dest.FuelConsumption, opt => opt.MapFrom(src => src.FuelConsumption))
                .ForMember(dest => dest.IsGasoline, opt => opt.MapFrom(src => src.IsGasoline))
                .ForMember(dest => dest.IsAutomatic, opt => opt.MapFrom(src => src.IsAutomatic))
                .ForMember(dest => dest.TermOfUse, opt => opt.MapFrom(src => src.TermOfUse))
                .ForMember(dest => dest.AdditionalFunction, opt => opt.MapFrom(src => src.AdditionalFunction))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.LicensePlate, opt => opt.MapFrom(src => src.LicensePlate))
                .ForMember(dest => dest.HouseNumberStreet, opt => opt.MapFrom(src => src.HouseNumberStreet))
                .ForMember(dest => dest.Ward, opt => opt.MapFrom(src => src.Ward))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District))
                .ForMember(dest => dest.CityProvince, opt => opt.MapFrom(src => src.CityProvince))
                .ForMember(dest => dest.CarImageFront, opt => opt.MapFrom(src => src.CarImageFront))
                .ForMember(dest => dest.CarImageBack, opt => opt.MapFrom(src => src.CarImageBack))
                .ForMember(dest => dest.CarImageLeft, opt => opt.MapFrom(src => src.CarImageLeft))
                .ForMember(dest => dest.CarImageRight, opt => opt.MapFrom(src => src.CarImageRight))
                .ForMember(dest => dest.InsuranceUri, opt => opt.MapFrom(src => src.InsuranceUri))
                .ForMember(dest => dest.InsuranceUriIsVerified, opt => opt.MapFrom(src => src.InsuranceUriIsVerified))
                .ForMember(dest => dest.RegistrationPaperUri, opt => opt.MapFrom(src => src.RegistrationPaperUri))
                .ForMember(dest => dest.RegistrationPaperUriIsVerified, opt => opt.MapFrom(src => src.RegistrationPaperUriIsVerified))
                .ForMember(dest => dest.CertificateOfInspectionUri, opt => opt.MapFrom(src => src.CertificateOfInspectionUri))
                .ForMember(dest => dest.CertificateOfInspectionUriIsVerified, opt => opt.MapFrom(src => src.CertificateOfInspectionUriIsVerified))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.NumberOfRides, opt => opt.Ignore())
                .ForMember(dest => dest.Rating, opt => opt.Ignore())
                .ForMember(dest => dest.TotalRating, opt => opt.Ignore());

            CreateMap<CarUpdateDTO, Car>()
                .ForMember(dest => dest.AccountId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<Car, CarVO_Full>();
        }
    }
}
