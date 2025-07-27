using CarRental_BE.Data;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.Wallet;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRental_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("balance/{accountId}")]
        public async Task<ApiResponse<WalletVO_Balance>> GetWalletBalance(Guid accountId)
        {
            try
            {
                var balance = await _walletService.GetWalletBalance(accountId);

                return new ApiResponse<WalletVO_Balance>(
                    status: 200,
                    message: "Wallet balance retrieved successfully",
                    data: balance);
            }
            catch (Exception ex)
            {
                return new ApiResponse<WalletVO_Balance>(
                    status: 500,
                    message: $"Error retrieving wallet balance: {ex.Message}",
                    data: null);
            }
        }

        [HttpGet("transactions/{accountId}")]
        public async Task<ApiResponse<PaginationResponse<WalletVO_Transaction>>> GetTransactionHistory(
            Guid accountId,
            [FromQuery] string? searchTerm,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] string? type,
            [FromQuery] string? status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filter = new TransactionFilterDTO
                {
                    SearchTerm = searchTerm,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Type = type,
                    Status = status
                };

                var request = new PaginationRequest
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _walletService.GetTransactionHistory(accountId, filter, request);

                return new ApiResponse<PaginationResponse<WalletVO_Transaction>>(
                    status: 200,
                    message: "Transaction history retrieved successfully",
                    data: result);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginationResponse<WalletVO_Transaction>>(
                    status: 500,
                    message: $"Error retrieving transaction history: {ex.Message}",
                    data: null);
            }
        }

        [HttpGet("transactions/{accountId}/{transactionId}")]
        public async Task<ApiResponse<WalletVO_TransactionDetail>> GetTransactionDetail(Guid accountId, Guid transactionId)
        {
            try
            {
                var transaction = await _walletService.GetTransactionDetail(transactionId, accountId);

                if (transaction == null)
                {
                    return new ApiResponse<WalletVO_TransactionDetail>(
                        status: 404,
                        message: "Transaction not found",
                        data: null);
                }

                return new ApiResponse<WalletVO_TransactionDetail>(
                    status: 200,
                    message: "Transaction detail retrieved successfully",
                    data: transaction);
            }
            catch (Exception ex)
            {
                return new ApiResponse<WalletVO_TransactionDetail>(
                    status: 500,
                    message: $"Error retrieving transaction detail: {ex.Message}",
                    data: null);
            }
        }

        [HttpPost("withdraw/{accountId}")]
        public async Task<ApiResponse<WalletVO_Transaction>> WithdrawMoney(Guid accountId, [FromBody] TransactionDTO withdrawDTO)
        {
            try
            {
                // Validate input
                if (withdrawDTO.Amount <= 0)
                {
                    return new ApiResponse<WalletVO_Transaction>(
                        status: 400,
                        message: "Amount must be greater than 0",
                        data: null);
                }

                var transaction = await _walletService.WithdrawMoney(accountId, withdrawDTO);

                return new ApiResponse<WalletVO_Transaction>(
                    status: 200,
                    message: "Money withdrawn successfully",
                    data: transaction);
            }
            catch (InvalidOperationException ex)
            {
                return new ApiResponse<WalletVO_Transaction>(
                    status: 400,
                    message: ex.Message,
                    data: null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<WalletVO_Transaction>(
                    status: 500,
                    message: $"Error processing withdrawal: {ex.Message}",
                    data: null);
            }
        }

        [HttpPost("topup/{accountId}")]
        public async Task<ApiResponse<WalletVO_Transaction>> TopupMoney(Guid accountId, [FromBody] TransactionDTO topupDTO)
        {
            try
            {
                // Validate input
                if (topupDTO.Amount <= 0)
                {
                    return new ApiResponse<WalletVO_Transaction>(
                        status: 400,
                        message: "Amount must be greater than 0",
                        data: null);
                }

                var transaction = await _walletService.TopupMoney(accountId, topupDTO);

                return new ApiResponse<WalletVO_Transaction>(
                    status: 200,
                    message: "Money topped up successfully",
                    data: transaction);
            }
            catch (InvalidOperationException ex)
            {
                return new ApiResponse<WalletVO_Transaction>(
                    status: 400,
                    message: ex.Message,
                    data: null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<WalletVO_Transaction>(
                    status: 500,
                    message: $"Error processing top-up: {ex.Message}",
                    data: null);
            }
        }

        [HttpPost("create/{accountId}")]
        public async Task<ApiResponse<WalletVO_Balance>> CreateWallet(Guid accountId)
        {
            try
            {
                var wallet = await _walletService.CreateWalletIfNotExists(accountId);

                return new ApiResponse<WalletVO_Balance>(
                    status: 201,
                    message: "Wallet created successfully",
                    data: wallet);
            }
            catch (InvalidOperationException ex)
            {
                return new ApiResponse<WalletVO_Balance>(
                    status: 400,
                    message: ex.Message,
                    data: null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<WalletVO_Balance>(
                    status: 500,
                    message: $"Error creating wallet: {ex.Message}",
                    data: null);
            }
        }
    }
}
