using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.VO.Wallet
{
    public record WalletVO_Balance
    {
        public Guid Id { get; set; }
        public long Balance { get; set; }
        public string FormattedBalance { get; set; } = string.Empty;
    }

    public record WalletVO_Transaction
    {
        public Guid Id { get; set; }
        public string? BookingNumber { get; set; }
        public long Amount { get; set; }
        public string FormattedAmount { get; set; } = string.Empty;
        public string? CarName { get; set; }
        public string? Message { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string FormattedDateTime { get; set; } = string.Empty;
        public string? Status { get; set; }
        public string? Type { get; set; }
    }

    public record WalletVO_TransactionHistory
    {
        public WalletVO_Balance Wallet { get; set; } = null!;
        public List<WalletVO_Transaction> Transactions { get; set; } = new();
        public int TotalTransactions { get; set; }
    }

    public record WalletVO_TransactionDetail
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public string? BookingNumber { get; set; }
        public long Amount { get; set; }
        public string FormattedAmount { get; set; } = string.Empty;
        public string? CarName { get; set; }
        public string? Message { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string FormattedDateTime { get; set; } = string.Empty;
        public string? Status { get; set; }
        public string? Type { get; set; }
        public string? BookingDetails { get; set; }
    }
}
