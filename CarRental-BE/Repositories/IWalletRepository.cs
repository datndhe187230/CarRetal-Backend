using CarRental_BE.Models.DTO;
using CarRental_BE.Models.NewEntities;

namespace CarRental_BE.Repositories
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetWalletByAccountId(Guid accountId);
        Task<Wallet> CreateWallet(Guid accountId);
        Task<(List<Transaction> transactions, int totalCount)> GetTransactionHistory(
            Guid walletId,
            TransactionFilterDTO filter,
            int pageNumber,
            int pageSize);
        Task<Transaction?> GetTransactionDetail(Guid transactionId, Guid accountId);
        Task<Transaction> CreateTransaction(Transaction transaction);
        Task<bool> UpdateWalletBalance(Guid walletId, decimal newBalance);
        Task<Transaction?> GetTransactionById(Guid transactionId);
        Task<Wallet?> GetAdminWallet(object accountId);
        Task<List<Transaction>> GetTransactionsByBookingNumberAsync(string bookingNumber);
        Task<bool> UpdateTransactionStatus(Guid transactionId, string status);
        Task<bool> UpdateTransaction(Transaction transaction);
        Task<string?> GetAdminWalletId();
    }
}
