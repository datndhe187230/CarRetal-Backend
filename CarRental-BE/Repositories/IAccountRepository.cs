using CarRental_BE.Models.NewEntities;

namespace CarRental_BE.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> getAccountByEmailWithRole(string email);
        Task<Account?> GetByIdAsync(Guid id);
        Task UpdateAsync(Account account);
        Task<(List<Account>, int)> GetAccountsWithPagingAsync(int page, int pageSize);
        Task ToggleAccountStatus(Guid accountId);
        Task<Account> GetAccountByIdAsync(Guid id);
        Task UpdateAccountAsync(Account account);
        Task<Account> GetCurrentUserAsync(Guid currentUserId);
        Task<Account?> CreateAccountAsync(Account newAccount, UserProfile newUserProfile, Wallet newWallet);
    }
}
