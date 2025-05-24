using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.Entities;

public partial class Feedback
{
    public string BookingNumber { get; set; } = null!;

    public string? Comment { get; set; }

    public int? Rating { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual Booking BookingNumberNavigation { get; set; } = null!;
}
