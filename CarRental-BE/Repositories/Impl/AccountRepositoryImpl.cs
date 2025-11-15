using CarRental_BE.Exceptions;
using Microsoft.EntityFrameworkCore;
using CarRental_BE.Models.NewEntities;
using CarRental_BE.Data;

namespace CarRental_BE.Repositories.Impl
{
    public class AccountRepositoryImpl : IAccountRepository
    {
        private readonly CarRentalContext _carRentalContext;

        public AccountRepositoryImpl(CarRentalContext carRentalContext)
        {
            _carRentalContext = carRentalContext;
        }

        public async Task<Account?> getAccountByEmailWithRole(string email)
        {
            var account = await _carRentalContext.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.Email == email);
            if (account == null) throw new UserNotFoundException(email);
            return account;
        }

        public async Task<(List<Account>, int)> GetAccountsWithPagingAsync(int page, int pageSize)
        {
            var query = _carRentalContext.Accounts.Include(a => a.Role).OrderByDescending(a => a.CreatedAt);
            int totalCount = await query.CountAsync();
            var items = await query.Skip((page -1) * pageSize).Take(pageSize).ToListAsync();
            return (items, totalCount);
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _carRentalContext.Accounts.FirstOrDefaultAsync(a => a.AccountId == id);
        }

        public async Task ToggleAccountStatus(Guid accountId)
        {
            var account = await _carRentalContext.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (account == null) return;
            account.IsActive = !account.IsActive;
            await _carRentalContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Account account)
        {
            _carRentalContext.Accounts.Update(account);
            await _carRentalContext.SaveChangesAsync();
        }

        public async Task<Account> GetAccountByIdAsync(Guid id)
        {
            return await _carRentalContext.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.AccountId == id);
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _carRentalContext.Accounts.Update(account);
            await _carRentalContext.SaveChangesAsync();
        }

        public async Task<Account> GetCurrentUserAsync(Guid currentUserId)
        {
            return await _carRentalContext.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.AccountId == currentUserId);
        }

        public async Task CreateAccountAsync(Account newAccount)
        {
            _carRentalContext.Accounts.Add(newAccount);
            await _carRentalContext.SaveChangesAsync();
        }
    }
}
