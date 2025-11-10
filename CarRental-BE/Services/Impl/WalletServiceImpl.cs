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
            var wallet = await _walletRepository.GetWalletByAccountId(accountId);

            if (wallet == null)
            {
                wallet = await _walletRepository.CreateWallet(accountId);
            }

            var walletVO = _mapper.Map<WalletVO_Balance>(wallet);
            walletVO.FormattedBalance = FormatCurrency(wallet.Balance);

            return walletVO;
        }

        public async Task<PaginationResponse<WalletVO_Transaction>> GetTransactionHistory(
            Guid accountId,
            TransactionFilterDTO filter,
            PaginationRequest request)
        {
            var wallet = await _walletRepository.GetWalletByAccountId(accountId);

            if (wallet == null)
            {
                return new PaginationResponse<WalletVO_Transaction>(
                    new List<WalletVO_Transaction>(), 0, request.PageSize, request.PageNumber);
            }

            var (transactions, totalCount) = await _walletRepository.GetTransactionHistory(
                wallet.Id, filter, request.PageNumber, request.PageSize);

            var transactionVOs = transactions.Select(t => {
                var vo = _mapper.Map<WalletVO_Transaction>(t);
                vo.FormattedAmount = FormatCurrency(t.Amount);
                vo.FormattedDateTime = FormatDateTime(t.CreatedAt);
                vo.CarName = t.CarName ?? "N/A";
                vo.BookingNumber = t.BookingNumber ?? "N/A";
                return vo;
            }).ToList();

            return new PaginationResponse<WalletVO_Transaction>(
                transactionVOs, totalCount, request.PageSize, request.PageNumber);
        }

        public async Task<WalletVO_TransactionDetail?> GetTransactionDetail(Guid transactionId, Guid accountId)
        {
            var transaction = await _walletRepository.GetTransactionDetail(transactionId, accountId);

            if (transaction == null)
            {
                return null;
            }

            var transactionVO = _mapper.Map<WalletVO_TransactionDetail>(transaction);
            transactionVO.FormattedAmount = FormatCurrency(transaction.Amount);
            transactionVO.FormattedDateTime = FormatDateTime(transaction.CreatedAt);
            transactionVO.CarName = transaction.CarName ?? "N/A";
            transactionVO.BookingNumber = transaction.BookingNumber ?? "N/A";
            Console.WriteLine("CarName (after mapping): " + transactionVO.CarName);

            return transactionVO;
        }

        public async Task<WalletVO_Transaction> WithdrawMoney(Guid accountId, TransactionDTO withdrawDTO)
        {
            var wallet = await _walletRepository.GetWalletByAccountId(accountId);

            if (wallet == null)
            {
                throw new InvalidOperationException("Wallet not found");
            }

            if (wallet.Balance < withdrawDTO.Amount)
            {
                throw new InvalidOperationException("Insufficient balance");
            }

            // Update wallet balance first
            await _walletRepository.UpdateWalletBalance(wallet.Id, wallet.Balance - withdrawDTO.Amount);

            // Create transaction record
            var transaction = await _walletRepository.CreateTransaction(new Models.Entities.Transaction
            {
                Id = Guid.NewGuid(),
                WalletId = wallet.Id,
                BookingNumber = withdrawDTO.BookingId,
                Amount = withdrawDTO.Amount,
                CarName = withdrawDTO.CarName,
                Message = withdrawDTO.Message ?? "Withdrawal",
                CreatedAt = DateTime.UtcNow,
                Status = (withdrawDTO.Status ?? TransactionStatus.Successful).ToString(),
                Type = (withdrawDTO.Type ?? TransactionType.Withdraw).ToString()
            });

            var transactionVO = _mapper.Map<WalletVO_Transaction>(transaction);
            transactionVO.FormattedAmount = FormatCurrency(transaction.Amount);
            transactionVO.FormattedDateTime = FormatDateTime(transaction.CreatedAt);

            return transactionVO;
        }

        public async Task<WalletVO_Transaction> TopupMoney(Guid accountId, TransactionDTO topupDTO)
        {
            var wallet = await _walletRepository.GetWalletByAccountId(accountId);

            if (wallet == null)
            {
                wallet = await _walletRepository.CreateWallet(accountId);
            }

            // Update wallet balance first
            await _walletRepository.UpdateWalletBalance(wallet.Id, wallet.Balance + topupDTO.Amount);

            // Create transaction record
            var transaction = await _walletRepository.CreateTransaction(new Models.Entities.Transaction
            {
                Id = Guid.NewGuid(),
                WalletId = wallet.Id,
                BookingNumber = topupDTO.BookingId,
                Amount = topupDTO.Amount,
                CarName = topupDTO.CarName,
                Message = topupDTO.Message ?? "Top-up",
                CreatedAt = DateTime.UtcNow,
                Status = (topupDTO.Status ?? TransactionStatus.Successful).ToString(),
                Type = (topupDTO.Type ?? TransactionType.Top_up).ToString()
            });


            var transactionVO = _mapper.Map<WalletVO_Transaction>(transaction);
            transactionVO.FormattedAmount = FormatCurrency(transaction.Amount);
            transactionVO.FormattedDateTime = FormatDateTime(transaction.CreatedAt);

            return transactionVO;
        }

        public async Task<WalletVO_Transaction> TopupMoneyAdmin(TransactionDTO topupDTO)
        {
            // Get admin account id from appsettings
            var adminAccountIdStr = _configuration["AdminAccountId"] ?? "9a2eb519-7054-4a1a-baed-a33dca077c37";
            if (string.IsNullOrEmpty(adminAccountIdStr))
            {
                throw new InvalidOperationException("Admin account id is not configured in appsettings.");
            }
            if (!Guid.TryParse(adminAccountIdStr, out var accountId))
            {
                throw new InvalidOperationException("Admin account id in appsettings is not a valid GUID.");
            }

            var wallet = await _walletRepository.GetWalletByAccountId(accountId);

            if (wallet == null)
            {
                wallet = await _walletRepository.CreateWallet(accountId);
            }

            // Update wallet balance first
            await _walletRepository.UpdateWalletBalance(wallet.Id, wallet.Balance + topupDTO.Amount);

            // Create transaction record
            var transaction = await _walletRepository.CreateTransaction(new Models.Entities.Transaction
            {
                Id = Guid.NewGuid(),
                WalletId = wallet.Id,
                BookingNumber = topupDTO.BookingId,
                Amount = topupDTO.Amount,
                CarName = topupDTO.CarName,
                Message = topupDTO.Message ?? "Top-up",
                CreatedAt = DateTime.UtcNow,
                Status = (topupDTO.Status ?? TransactionStatus.Successful).ToString(),
                Type = (topupDTO.Type ?? TransactionType.Top_up).ToString()
            });

            var transactionVO = _mapper.Map<WalletVO_Transaction>(transaction);
            transactionVO.FormattedAmount = FormatCurrency(transaction.Amount);
            transactionVO.FormattedDateTime = FormatDateTime(transaction.CreatedAt);

            return transactionVO;
        }

        public async Task<WalletVO_Balance> CreateWalletIfNotExists(Guid accountId)
        {
            var wallet = await _walletRepository.GetWalletByAccountId(accountId);

            if (wallet == null)
            {
                wallet = await _walletRepository.CreateWallet(accountId);
            }

            var walletVO = _mapper.Map<WalletVO_Balance>(wallet);
            walletVO.FormattedBalance = FormatCurrency(wallet.Balance);

            return walletVO;
        }

        public async Task<bool> RevertBookingTransactionsAsync(string bookingNumber)
        {
            // Find all transactions for this booking
            var transactions = await _walletRepository.GetTransactionsByBookingNumberAsync(bookingNumber);
            bool anyReverted = false;

            foreach (var transaction in transactions)
            {
                if(transaction.Status != TransactionStatus.Processing.ToString())
                {
                    // Skip transactions that are not in processing state
                    continue;
                }
                var wallet = await _walletRepository.GetWalletByAccountId(transaction.Wallet.IdNavigation.Id);
                
                if (transaction.Type == TransactionType.Withdraw.ToString())
                {
                    // Refund the withdrawn amount
                    await _walletRepository.UpdateWalletBalance(wallet.Id, wallet.Balance + transaction.Amount);
                }
                else if (transaction.Type == TransactionType.Top_up.ToString() ||
                         transaction.Type == TransactionType.receive_deposit.ToString())
                {
                    // Deduct the top-up amount
                    await _walletRepository.UpdateWalletBalance(wallet.Id, wallet.Balance - transaction.Amount);
                }
                // Mark transaction as reverted;
                await _walletRepository.UpdateTransactionStatus(transaction.Id, TransactionStatus.Failed.ToString());

                anyReverted = true;
            }

            return anyReverted;
        }

        public async Task<bool> UpdateTransactionStatusAsync(Guid transactionId, string status)
        {
            var transaction = await _walletRepository.GetTransactionById(transactionId);

            if (transaction == null)
            {
                throw new InvalidOperationException("Transaction not found");
            }

            // Update the transaction status
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
    }
}
