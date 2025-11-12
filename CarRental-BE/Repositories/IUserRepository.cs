using CarRental_BE.Models.DTO;
using CarRental_BE.Models.NewEntities;

namespace CarRental_BE.Repositories
{
    public interface IUserRepository
    {
        Task<UserProfile?> GetById(Guid id);
        Task<UserProfile?> UpdateUserProfile(Guid id, UserUpdateDTO dto);

        Task<bool> ChangePassword(Guid id, ChangePasswordDTO dto);

        Task<bool> Register(RegisterDTO dto);

        Task<string?> GetUserProfileFullNameByAccountId(Guid accountId);

        Task ResetPassword(ChangePasswordDTO dto);
    }
}
