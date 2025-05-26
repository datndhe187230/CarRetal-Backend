using CarRental_BE.Models.Entities;

namespace CarRental_BE.Repositories
{
    public interface IUserRepository
    {
        Task<UserProfile?> GetById(Guid id);
    }
}
