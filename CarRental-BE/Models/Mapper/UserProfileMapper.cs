using CarRental_BE.Models.NewEntities;
using CarRental_BE.Models.VO.User;  

namespace CarRental_BE.Models.Mapper
{
    public static class UserProfileMapper
    {
        public static UserProfileVO ToVO(UserProfile entity)
        {
            return new UserProfileVO
            {
                Id = entity.AccountId,
                FullName = entity.FullName,
                Dob = entity.Dob,
                PhoneNumber = entity.PhoneNumber,
                NationalId = entity.NationalId,
                DrivingLicenseUri = entity.DrivingLicenseUri,
                HouseNumberStreet = entity.Address.HouseNumberStreet,
                Ward = entity.Address.Ward,
                District = entity.Address.District,
                CityProvince = entity.Address.CityProvince,
                Email = entity.Account.Email
            };
        }
    }
}
