using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.User;

namespace CarRental_BE.Services
{
    public interface IUserService
    {
        Task<UserProfileVO?> GetUserProfile(Guid id);
        Task<UserProfileVO?> UpdateUserProfile(Guid id, UserUpdateDTO dto);
        Task<bool> ChangePassword(Guid id, ChangePasswordDTO dto);
    }
}
