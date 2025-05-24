using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.Entities;

public partial class Transaction
{
    public Guid Id { get; set; }

    public Guid WalletId { get; set; }

    public string? BookingNumber { get; set; }

    public long Amount { get; set; }

    public string? CarName { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Status { get; set; }

    public string? Type { get; set; }

    public virtual Booking? BookingNumberNavigation { get; set; }

    public virtual Wallet Wallet { get; set; } = null!;
}
