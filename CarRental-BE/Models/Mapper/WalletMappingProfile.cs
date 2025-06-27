using CarRental_BE.Models.Entities;
using CarRental_BE.Models.VO.Wallet;
using AutoMapper;
 
namespace CarRental_BE.Models.Mapper
{
    public class WalletMappingProfile : Profile
    {
        public WalletMappingProfile()
        {
            CreateMap<Wallet, WalletVO_Balance>();

            CreateMap<Transaction, WalletVO_Transaction>()
                .ForMember(dest => dest.FormattedAmount, opt => opt.Ignore())
                .ForMember(dest => dest.FormattedDateTime, opt => opt.Ignore());

            CreateMap<Transaction, WalletVO_TransactionDetail>()
                .ForMember(dest => dest.FormattedAmount, opt => opt.Ignore())
                .ForMember(dest => dest.FormattedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.BookingDetails, opt => opt.MapFrom(src =>
                    src.BookingNumberNavigation != null ?
                    $"{src.BookingNumberNavigation.PickUpLocation} - {src.BookingNumberNavigation.DropOffLocation}" :
                    null));
        }
    }
}
