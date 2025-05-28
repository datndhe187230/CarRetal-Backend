using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VOs.User;

namespace CarRental_BE.Services
{
    public interface IUserService
    {
        Task<UserProfileVO?> GetUserProfile(Guid id);
        Task<UserProfileVO?> UpdateUserProfile(Guid id, UserUpdateDTO dto);

    }
}
