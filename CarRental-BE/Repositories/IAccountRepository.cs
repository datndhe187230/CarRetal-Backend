using CarRental_BE.Models.Entities;

namespace CarRental_BE.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> getAccountByEmailWithRole(string email);
    }
}
