using CarRental_BE.Models.DTO;
using Microsoft.EntityFrameworkCore;
using CarRental_BE.Models.NewEntities;
using CarRental_BE.Data;

namespace CarRental_BE.Repositories.Impl
{
    public class WalletRepositoryImpl : IWalletRepository
    {
        private readonly CarRentalContext _context;

        public WalletRepositoryImpl(CarRentalContext context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetWalletByAccountId(Guid accountId)
        {
            return await _context.Wallets
                .Include(w => w.Account)
                .FirstOrDefaultAsync(w => w.AccountId == accountId);
        }

        public async Task<Wallet> CreateWallet(Guid accountId)
        {
            // Check if account exists
            var accountExists = await _context.Accounts.AnyAsync(a => a.AccountId == accountId);
            if (!accountExists)
            {
                throw new InvalidOperationException($"Account with ID {accountId} does not exist");
            }

            // Check if wallet already exists
            var existing = await GetWalletByAccountId(accountId);
            if (existing != null)
            {
                return existing;
            }

            var wallet = new Wallet
            {
                AccountId = accountId,
                BalanceCents = 0m,
                LockedCents = 0m,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();

            return wallet;
        }

        public async Task<(List<Transaction> transactions, int totalCount)> GetTransactionHistory(
            Guid walletId,
            TransactionFilterDTO filter,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Transactions
                .Where(t => t.WalletId == walletId)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var s = filter.SearchTerm.ToLower();
                query = query.Where(t =>
                    (t.Type != null && t.Type.ToLower().Contains(s)) ||
                    (t.Description != null && t.Description.ToLower().Contains(s)) ||
                    (t.BookingNumber != null && t.BookingNumber.ToLower().Contains(s)));
            }

            // Apply date filters
            if (filter.FromDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                var to = filter.ToDate.Value.Date.AddDays(1); // Include the entire day
                query = query.Where(t => t.CreatedAt < to);
            }

            // Apply type filter
            if (!string.IsNullOrEmpty(filter.Type))
            {
                query = query.Where(t => t.Type == filter.Type);
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(filter.Status))
            {
                query = query.Where(t => t.Status == filter.Status);
            }

            // Order by creation date (newest first)
            query = query.OrderByDescending(t => t.CreatedAt);

            var total = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<Transaction?> GetTransactionDetail(Guid transactionId, Guid accountId)
        {
            return await _context.Transactions
                .Include(t => t.Wallet)
                .Include(t => t.BookingNumberNavigation)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId && t.Wallet.AccountId == accountId);
        }

        public async Task<Transaction> CreateTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<bool> UpdateWalletBalance(Guid walletId, decimal newBalance)
        {
            var wallet = await _context.Wallets.FindAsync(walletId);
            if (wallet == null)
            {
                throw new InvalidOperationException($"Wallet with ID {walletId} not found");
            }

            wallet.BalanceCents = newBalance;
            wallet.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Transaction?> GetTransactionById(Guid transactionId)
        {
            return await _context.Transactions
                .Include(t => t.Wallet)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }

        public async Task<Wallet?> GetAdminWallet(object accountId)
        {
            Guid id = accountId is Guid g ? g : (accountId is string s && Guid.TryParse(s, out var parsed) ? parsed : Guid.Empty);
            if (id == Guid.Empty) throw new ArgumentException("Invalid accountId type");
            var account = await _context.Accounts.Include(a => a.Wallet).FirstOrDefaultAsync(a => a.AccountId == id && a.RoleId == 1);
            return account?.Wallet;
        }

        public async Task<List<Transaction>> GetTransactionsByBookingNumberAsync(string bookingNumber)
        {
            return await _context.Transactions
                .Where(t => t.BookingNumber == bookingNumber)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateTransactionStatus(Guid transactionId, string status)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);
            if (transaction == null) return false;
            transaction.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTransaction(Transaction transaction)
        {
            var existing = await _context.Transactions.FindAsync(transaction.TransactionId);
            if (existing == null) return false;
            _context.Entry(existing).CurrentValues.SetValues(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<string?> GetAdminWalletId()
        {
            return _context.Accounts
                .Where(a => a.RoleId == 1)
                .Select(a => a.AccountId.ToString())
                .FirstOrDefaultAsync();
        }
    }
}
