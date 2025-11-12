using AutoMapper;
using WalletEntity = CarRental_BE.Models.NewEntities.Wallet;
using TransactionEntity = CarRental_BE.Models.NewEntities.Transaction;
using CarRental_BE.Models.VO.Wallet;
 
namespace CarRental_BE.Models.Mapper
{
    public class WalletMappingProfile : Profile
    {
        public WalletMappingProfile()
        {
            CreateMap<WalletEntity, WalletVO_Balance>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => (long)src.BalanceCents));

            CreateMap<TransactionEntity, WalletVO_Transaction>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.BookingNumber, opt => opt.MapFrom(src => src.BookingNumber))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => (long)src.AmountCents))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.FormattedAmount, opt => opt.Ignore())
                .ForMember(dest => dest.FormattedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.CarName, opt => opt.MapFrom(src => src.BookingNumberNavigation.Car.Brand + " " + src.BookingNumberNavigation.Car.Model));

            CreateMap<TransactionEntity, WalletVO_TransactionDetail>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.WalletId, opt => opt.MapFrom(src => src.WalletId))
                .ForMember(dest => dest.BookingNumber, opt => opt.MapFrom(src => src.BookingNumber))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => (long)src.AmountCents))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.FormattedAmount, opt => opt.Ignore())
                .ForMember(dest => dest.FormattedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src => src.BookingNumberNavigation != null ? (src.BookingNumberNavigation.PickUpAddress.CityProvince + " - " + src.BookingNumberNavigation.DropOffAddress.CityProvince) : null));
        }
    }
}
