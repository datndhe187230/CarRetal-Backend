using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.NewEntities;

public partial class BookingStatusHistory
{
    public long Id { get; set; }

    public string BookingNumber { get; set; } = null!;

    public string? OldStatus { get; set; }

    public string NewStatus { get; set; } = null!;

    public string? Note { get; set; }

    public string? PictureUrl { get; set; }

    public DateTime ChangedAt { get; set; }

    public virtual Booking BookingNumberNavigation { get; set; } = null!;
}
