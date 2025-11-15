using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.Entities;

public partial class Review
{
    public Guid ReviewId { get; set; }

    public string BookingNumber { get; set; } = null!;

    public Guid FromAccountId { get; set; }

    public Guid ToAccountId { get; set; }

    public byte Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Booking BookingNumberNavigation { get; set; } = null!;

    public virtual Account FromAccount { get; set; } = null!;

    public virtual Account ToAccount { get; set; } = null!;
}
