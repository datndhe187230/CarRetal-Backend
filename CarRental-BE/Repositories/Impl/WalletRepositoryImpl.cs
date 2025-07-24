using CarRental_BE.Data;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using CarRental_BE.Repositories;
using Microsoft.EntityFrameworkCore;

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
                .Include(w => w.IdNavigation)
                .FirstOrDefaultAsync(w => w.Id == accountId);
        }


        public async Task<Wallet> CreateWallet(Guid accountId)
        {
            // Check if account exists
            var accountExists = await _context.Accounts.AnyAsync(a => a.Id == accountId);
            if (!accountExists)
            {
                throw new InvalidOperationException($"Account with ID {accountId} does not exist");
            }

            // Check if wallet already exists
            var existingWallet = await GetWalletByAccountId(accountId);
            if (existingWallet != null)
            {
                return existingWallet;
            }

            var wallet = new Wallet
            {
                Id = accountId,
                Balance = 0
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
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(t =>
                    (t.Type != null && t.Type.ToLower().Contains(searchTerm)) ||
                    (t.CarName != null && t.CarName.ToLower().Contains(searchTerm)) ||
                    (t.BookingNumber != null && t.BookingNumber.Contains(searchTerm)) ||
                    (t.Message != null && t.Message.ToLower().Contains(searchTerm)));
            }

            // Apply date filters
            if (filter.FromDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                var toDate = filter.ToDate.Value.Date.AddDays(1); // Include the entire day
                query = query.Where(t => t.CreatedAt < toDate);
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

            var totalCount = await query.CountAsync();

            var transactions = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (transactions, totalCount);
        }

        public async Task<Transaction?> GetTransactionDetail(Guid transactionId, Guid accountId)
        {
            return await _context.Transactions
                .Include(t => t.Wallet)
                .Include(t => t.BookingNumberNavigation)
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.Wallet.Id == accountId);
        }

        public async Task<Transaction> CreateTransaction(Transaction transaction)
        {
            try
            {
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                return transaction;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create transaction: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateWalletBalance(Guid walletId, long newBalance)
        {
            try
            {
                var wallet = await _context.Wallets.FindAsync(walletId);
                if (wallet == null)
                {
                    throw new InvalidOperationException($"Wallet with ID {walletId} not found");
                }

                wallet.Balance = newBalance;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to update wallet balance: {ex.Message}", ex);
            }
        }

        public async Task<Transaction?> GetTransactionById(Guid transactionId)
        {
            return await _context.Transactions
                .Include(t => t.Wallet)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }

    }
}
