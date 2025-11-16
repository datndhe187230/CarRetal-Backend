using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.Wallet;
using CarRental_BE.Models.Enum;

namespace CarRental_BE.Services
{
    public interface IWalletService
    {
        Task<WalletVO_Balance> GetWalletBalance(Guid accountId);
        Task<PaginationResponse<WalletVO_Transaction>> GetTransactionHistory(
            Guid accountId,
            TransactionFilterDTO filter,
            PaginationRequest request);
        Task<WalletVO_TransactionDetail?> GetTransactionDetail(Guid transactionId, Guid accountId);
        Task<WalletVO_Transaction> WithdrawMoney(Guid accountId, TransactionDTO withdrawDTO);
        Task<WalletVO_Transaction> TopupMoney(Guid accountId, TransactionDTO topupDTO);
        Task<WalletVO_Balance> CreateWalletIfNotExists(Guid accountId);
        Task<WalletVO_Transaction> TopupMoneyAdmin(TransactionDTO topupDTO);
        Task<bool> RevertBookingTransactionsAsync(string bookingNumber);
        Task<bool> UpdateTransactionStatusAsync(Guid transactionId, string status);
        Task<bool> RevertBookingTransactionsByTypeAsync(string bookingNumber, TransactionType type);
    }
}
