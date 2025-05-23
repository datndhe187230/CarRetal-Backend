using System;
using System.Collections.Generic;

namespace CarRental_BE.Models;

public partial class Wallet
{
    public Guid Id { get; set; }

    public long Balance { get; set; }

    public virtual Account IdNavigation { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
