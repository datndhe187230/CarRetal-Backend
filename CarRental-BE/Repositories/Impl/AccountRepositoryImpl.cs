using CarRental_BE.Data;
using CarRental_BE.Exceptions;
using CarRental_BE.Models.Entities;
using Microsoft.EntityFrameworkCore;

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
            var account = await _carRentalContext.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Email == email);

            //if (account == null)
            //    throw new UserNotFoundException(email);

            return account;
        }

        public async Task<(List<Account>, int)> GetAccountsWithPagingAsync(int page, int pageSize)
        {
            var query = _carRentalContext.Accounts
                .Include(a => a.Role)
                .OrderByDescending(a => a.CreatedAt);

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _carRentalContext.Accounts
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task ToggleAccountStatus(Guid accountId)
        {
            _carRentalContext.Accounts
                .Where(a => a.Id == accountId)
                .ExecuteUpdate(a => a.SetProperty(ac => ac.IsActive, ac => !ac.IsActive));
            await _carRentalContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Account account)
        {
            _carRentalContext.Accounts.Update(account);
            await _carRentalContext.SaveChangesAsync();
        }

        public async Task<Account> GetAccountByIdAsync(Guid id)
        {
            return await _carRentalContext.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _carRentalContext.Accounts.Update(account);
            await _carRentalContext.SaveChangesAsync();
        }

        public async Task<Account> GetCurrentUserAsync(Guid currentUserId)
        {
            return await _carRentalContext.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Id == currentUserId);
        }

        public async Task CreateAccountAsync(Account account, UserProfile userProfile, Wallet wallet)
        {
            await _carRentalContext.Accounts.AddAsync(account);
            await _carRentalContext.UserProfiles.AddAsync(userProfile);
            await _carRentalContext.Wallets.AddAsync(wallet);
            await _carRentalContext.SaveChangesAsync();
        }
    }
}
