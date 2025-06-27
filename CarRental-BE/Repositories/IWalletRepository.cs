using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;

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
        Task<bool> UpdateWalletBalance(Guid walletId, long newBalance);
        Task<Transaction?> GetTransactionById(Guid transactionId);
    }
}
