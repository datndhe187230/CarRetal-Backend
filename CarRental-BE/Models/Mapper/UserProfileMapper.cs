using CarRental_BE.Models.Entities;
using CarRental_BE.Models.VOs.User;

namespace CarRental_BE.Models.Mapper
{
    public static class UserProfileMapper
    {
        public static UserProfileVO ToVO(UserProfile entity)
        {
            return new UserProfileVO
            {
                Id = entity.Id,
                FullName = entity.FullName,
                Dob = entity.Dob,
                PhoneNumber = entity.PhoneNumber,
                NationalId = entity.NationalId,
                DrivingLicenseUri = entity.DrivingLicenseUri,
                HouseNumberStreet = entity.HouseNumberStreet,
                Ward = entity.Ward,
                District = entity.District,
                CityProvince = entity.CityProvince
            };
        }
    }
}
