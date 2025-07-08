using CarRental_BE.Models.Entities;

namespace CarRental_BE.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> getAccountByEmailWithRole(string email);
        Task<Account?> GetByIdAsync(Guid id);
        Task UpdateAsync(Account account);
        Task<(List<Account>, int)> GetAccountsWithPagingAsync(int page, int pageSize);
        Task ToggleAccountStatus(Guid accountId);
    }
}
