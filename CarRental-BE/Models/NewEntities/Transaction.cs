using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.NewEntities;

public partial class Transaction
{
    public Guid TransactionId { get; set; }

    public Guid WalletId { get; set; }

    public string? BookingNumber { get; set; }

    public decimal AmountCents { get; set; }

    public string Type { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Booking? BookingNumberNavigation { get; set; }

    public virtual Wallet Wallet { get; set; } = null!;
}
