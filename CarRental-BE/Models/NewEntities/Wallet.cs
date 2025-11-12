using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.NewEntities;

public partial class Wallet
{
    public Guid AccountId { get; set; }

    public decimal BalanceCents { get; set; }

    public decimal LockedCents { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
