using CarRental_BE.Data;
using CarRental_BE.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarRental_BE.Repositories.Impl
{
    public class AccountRepository : IAccountRepository
    {
        private readonly CarRentalContext _carRentalContext;

        public AccountRepository(CarRentalContext carRentalContext)
        {
            _carRentalContext = carRentalContext;
        }

        public async Task<Account?> getAccountByEmailWithRole(string email)
        {
            return await _carRentalContext.Accounts
                                      .Include(a => a.Role)
                                      .FirstOrDefaultAsync(a => a.Email == email);
        }
    }
}
