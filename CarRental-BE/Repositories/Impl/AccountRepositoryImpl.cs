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

            if (account == null)
                throw new UserNotFoundException(email);

            return account;
        }

        
        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _carRentalContext.Accounts
                .FirstOrDefaultAsync(a => a.Id == id);
        }

       
        public async Task UpdateAsync(Account account)
        {
            _carRentalContext.Accounts.Update(account);
            await _carRentalContext.SaveChangesAsync();
        }
    }
}
