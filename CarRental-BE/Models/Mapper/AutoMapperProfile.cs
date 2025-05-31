using AutoMapper;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.VO.Car;
using CarRental_BE.Models.VO;

namespace CarRental_BE.Models.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Car, CarVO_ViewACar>()
                // Chỉ map các trường cần thiết
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
        }
    }
}
