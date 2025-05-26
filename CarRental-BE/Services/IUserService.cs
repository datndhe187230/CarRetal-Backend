using CarRental_BE.Models.VOs.User;

namespace CarRental_BE.Services
{
    public interface IUserService
    {
        Task<UserProfileVO?> GetUserProfile(Guid id);
    }
}
