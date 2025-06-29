using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.Wallet;

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
        Task<WalletVO_Transaction> WithdrawMoney(Guid accountId, WithdrawDTO withdrawDTO);
        Task<WalletVO_Transaction> TopupMoney(Guid accountId, TopupDTO topupDTO);
        Task<WalletVO_Balance> CreateWalletIfNotExists(Guid accountId);
    }
}
