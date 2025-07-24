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

        public WalletServiceImpl(IWalletRepository walletRepository, IMapper mapper)
        {
            _walletRepository = walletRepository;
            _mapper = mapper;
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

        public async Task<WalletVO_Transaction> WithdrawMoney(Guid accountId, WithdrawDTO withdrawDTO)
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
                BookingNumber = null,
                Amount = withdrawDTO.Amount,
                CarName = null,
                Message = withdrawDTO.Message ?? "Withdrawal",
                CreatedAt = DateTime.UtcNow,
                Status = TransactionStatus.Successful.ToString(),
                Type = TransactionType.Withdraw.ToString()
            });

            var transactionVO = _mapper.Map<WalletVO_Transaction>(transaction);
            transactionVO.FormattedAmount = FormatCurrency(transaction.Amount);
            transactionVO.FormattedDateTime = FormatDateTime(transaction.CreatedAt);

            return transactionVO;
        }

        public async Task<WalletVO_Transaction> TopupMoney(Guid accountId, TopupDTO topupDTO)
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
                BookingNumber = null,
                Amount = topupDTO.Amount,
                CarName = null,
                Message = topupDTO.Message ?? "Top-up",
                CreatedAt = DateTime.UtcNow,
                Status = TransactionStatus.Successful.ToString(),
                Type = TransactionType.Top_up.ToString()
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
