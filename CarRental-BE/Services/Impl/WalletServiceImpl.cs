using AutoMapper;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.Wallet;
using CarRental_BE.Repositories;
using CarRental_BE.Services;
using CarRental_BE.Exceptions;
using System.Globalization;
using InvalidOperationException = CarRental_BE.Exceptions.InvalidOperationException;
using CarRental_BE.Models.Enum;
using CarRental_BE.Models.NewEntities;

namespace CarRental_BE.Services.Impl
{
    public class WalletServiceImpl : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public WalletServiceImpl(IWalletRepository walletRepository, IMapper mapper, IConfiguration configuration)
        {
            _walletRepository = walletRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<WalletVO_Balance> GetWalletBalance(Guid accountId)
        {
            var wallet = await _walletRepository.GetWalletByAccountId(accountId) ?? await _walletRepository.CreateWallet(accountId);
            var walletVO = _mapper.Map<WalletVO_Balance>(wallet);
            walletVO.FormattedBalance = FormatCurrency((long)wallet.BalanceCents);
            return walletVO;
        }

        public async Task<PaginationResponse<WalletVO_Transaction>> GetTransactionHistory(Guid accountId, TransactionFilterDTO filter, PaginationRequest request)
        {
            var wallet = await _walletRepository.GetWalletByAccountId(accountId);
            if (wallet == null)
            {
                return new PaginationResponse<WalletVO_Transaction>(new List<WalletVO_Transaction>(),0, request.PageSize, request.PageNumber);
            }
            var (transactions, totalCount) = await _walletRepository.GetTransactionHistory(wallet.AccountId, filter, request.PageNumber, request.PageSize);
            var transactionVOs = transactions.Select(t => {
                var vo = _mapper.Map<WalletVO_Transaction>(t);
                vo.FormattedAmount = FormatCurrency((long)t.AmountCents);
                vo.FormattedDateTime = FormatDateTime(t.CreatedAt);
                vo.CarName = vo.CarName ?? "N/A";
                vo.BookingNumber = vo.BookingNumber ?? "N/A";
                return vo;
            }).ToList();
            return new PaginationResponse<WalletVO_Transaction>(transactionVOs, totalCount, request.PageSize, request.PageNumber);
        }

        public async Task<WalletVO_TransactionDetail?> GetTransactionDetail(Guid transactionId, Guid accountId)
        {
            var transaction = await _walletRepository.GetTransactionDetail(transactionId, accountId);
            if (transaction == null) return null;
            var transactionVO = _mapper.Map<WalletVO_TransactionDetail>(transaction);
            transactionVO.FormattedAmount = FormatCurrency((long)transaction.AmountCents);
            transactionVO.FormattedDateTime = FormatDateTime(transaction.CreatedAt);
            transactionVO.CarName = transactionVO.CarName ?? "N/A";
            transactionVO.BookingNumber = transactionVO.BookingNumber ?? "N/A";
            return transactionVO;
        }

        public async Task<WalletVO_Transaction> WithdrawMoney(Guid accountId, TransactionDTO withdrawDTO)
        {
            var wallet = await _walletRepository.GetWalletByAccountId(accountId);
            if (wallet == null) throw new InvalidOperationException("Wallet not found");
            if (wallet.BalanceCents < withdrawDTO.Amount) throw new InvalidOperationException("Insufficient balance");
            await _walletRepository.UpdateWalletBalance(wallet.AccountId, wallet.BalanceCents - withdrawDTO.Amount);
            var transaction = await _walletRepository.CreateTransaction(new Transaction
            {
                TransactionId = Guid.NewGuid(),
                WalletId = wallet.AccountId,
                BookingNumber = withdrawDTO.BookingId,
                AmountCents = withdrawDTO.Amount,
                Description = withdrawDTO.Message ?? "Withdrawal",
                CreatedAt = DateTime.UtcNow,
                Status = (withdrawDTO.Status ?? TransactionStatus.Successful).ToString(),
                Type = (withdrawDTO.Type ?? TransactionType.Withdraw).ToString()
            });
            var transactionVO = _mapper.Map<WalletVO_Transaction>(transaction);
            transactionVO.FormattedAmount = FormatCurrency((long)transaction.AmountCents);
            transactionVO.FormattedDateTime = FormatDateTime(transaction.CreatedAt);
            return transactionVO;
        }

        public async Task<WalletVO_Transaction> TopupMoney(Guid accountId, TransactionDTO topupDTO)
        {
            var wallet = await _walletRepository.GetWalletByAccountId(accountId) ?? await _walletRepository.CreateWallet(accountId);
            await _walletRepository.UpdateWalletBalance(wallet.AccountId, wallet.BalanceCents + topupDTO.Amount);
            var transaction = await _walletRepository.CreateTransaction(new Transaction
            {
                TransactionId = Guid.NewGuid(),
                WalletId = wallet.AccountId,
                BookingNumber = topupDTO.BookingId,
                AmountCents = topupDTO.Amount,
                Description = topupDTO.Message ?? "Top-up",
                CreatedAt = DateTime.UtcNow,
                Status = (topupDTO.Status ?? TransactionStatus.Successful).ToString(),
                Type = (topupDTO.Type ?? TransactionType.Top_up).ToString()
            });
            var transactionVO = _mapper.Map<WalletVO_Transaction>(transaction);
            transactionVO.FormattedAmount = FormatCurrency((long)transaction.AmountCents);
            transactionVO.FormattedDateTime = FormatDateTime(transaction.CreatedAt);
            return transactionVO;
        }

        public async Task<WalletVO_Transaction> TopupMoneyAdmin(TransactionDTO topupDTO)
        {
            var accountId = await getAdminWalletIdAsync();
            var wallet = await _walletRepository.GetWalletByAccountId(accountId) ?? await _walletRepository.CreateWallet(accountId);
            await _walletRepository.UpdateWalletBalance(wallet.AccountId, wallet.BalanceCents + topupDTO.Amount);
            var transaction = await _walletRepository.CreateTransaction(new Transaction
            {
                TransactionId = Guid.NewGuid(),
                WalletId = wallet.AccountId,
                BookingNumber = topupDTO.BookingId,
                AmountCents = topupDTO.Amount,
                Description = topupDTO.Message ?? "Top-up",
                CreatedAt = DateTime.UtcNow,
                Status = (topupDTO.Status ?? TransactionStatus.Successful).ToString(),
                Type = (topupDTO.Type ?? TransactionType.Top_up).ToString()
            });
            var transactionVO = _mapper.Map<WalletVO_Transaction>(transaction);
            transactionVO.FormattedAmount = FormatCurrency((long)transaction.AmountCents);
            transactionVO.FormattedDateTime = FormatDateTime(transaction.CreatedAt);
            return transactionVO;
        }

        public async Task<WalletVO_Balance> CreateWalletIfNotExists(Guid accountId)
        {
            var wallet = await _walletRepository.GetWalletByAccountId(accountId) ?? await _walletRepository.CreateWallet(accountId);
            var walletVO = _mapper.Map<WalletVO_Balance>(wallet);
            walletVO.FormattedBalance = FormatCurrency((long)wallet.BalanceCents);
            return walletVO;
        }

        public async Task<bool> RevertBookingTransactionsAsync(string bookingNumber)
        {
            var transactions = await _walletRepository.GetTransactionsByBookingNumberAsync(bookingNumber);
            bool anyReverted = false;
            foreach (var transaction in transactions)
            {
                if (transaction.Status != TransactionStatus.Processing.ToString()) continue;
                var wallet = await _walletRepository.GetWalletByAccountId(transaction.WalletId);
                if (transaction.Type == TransactionType.Withdraw.ToString())
                {
                    await _walletRepository.UpdateWalletBalance(wallet.AccountId, wallet.BalanceCents + transaction.AmountCents);
                }
                else if (transaction.Type == TransactionType.Top_up.ToString() || transaction.Type == TransactionType.receive_deposit.ToString())
                {
                    await _walletRepository.UpdateWalletBalance(wallet.AccountId, wallet.BalanceCents - transaction.AmountCents);
                }
                await _walletRepository.UpdateTransactionStatus(transaction.TransactionId, TransactionStatus.Failed.ToString());
                anyReverted = true;
            }
            return anyReverted;
        }

        public async Task<bool> RevertBookingTransactionsByTypeAsync(string bookingNumber, TransactionType type)
        {
            var transactions = await _walletRepository.GetTransactionsByBookingNumberAsync(bookingNumber);
            bool anyReverted = false;
            foreach (var transaction in transactions)
            {
                if (transaction.Status != TransactionStatus.Processing.ToString()) continue;
                if (!string.Equals(transaction.Type, type.ToString(), StringComparison.OrdinalIgnoreCase)) continue;
                var wallet = await _walletRepository.GetWalletByAccountId(transaction.WalletId);
                if (transaction.Type == TransactionType.Withdraw.ToString() || transaction.Type == TransactionType.offset_final_payment.ToString())
                {
                    // For remaining payment withdraw from renter, put the money back
                    await _walletRepository.UpdateWalletBalance(wallet.AccountId, wallet.BalanceCents + transaction.AmountCents);
                }
                else
                {
                    // For counterpart topups (owner/admin), remove the previously added amount
                    await _walletRepository.UpdateWalletBalance(wallet.AccountId, wallet.BalanceCents - transaction.AmountCents);
                }
                await _walletRepository.UpdateTransactionStatus(transaction.TransactionId, TransactionStatus.Failed.ToString());
                anyReverted = true;
            }
            return anyReverted;
        }

        public async Task<bool> UpdateTransactionStatusAsync(Guid transactionId, string status)
        {
            var transaction = await _walletRepository.GetTransactionById(transactionId);
            if (transaction == null) throw new InvalidOperationException("Transaction not found");
            transaction.Status = status;
            await _walletRepository.UpdateTransaction(transaction);
            return true;
        }

        private string FormatCurrency(long amount)
        {
            return amount.ToString("N0", new CultureInfo("vi-VN")) + " VND";
        }

        private string FormatDateTime(DateTime? dateTime)
        {
            if (!dateTime.HasValue) return string.Empty;
            return dateTime.Value.ToString("dd/MM/yyyy HH:mm", new CultureInfo("vi-VN"));
        }

        private async Task<Guid> getAdminWalletIdAsync()
        {
            var adminAccountIdStr = _configuration["AdminAccountId"];
            if (string.IsNullOrEmpty(adminAccountIdStr))
            {
                adminAccountIdStr = await _walletRepository.GetAdminWalletId();
            }
            if (!Guid.TryParse(adminAccountIdStr, out var accountId)) throw new InvalidOperationException("Admin account id is not a valid GUID.");
            return accountId;
        }
    }
}
