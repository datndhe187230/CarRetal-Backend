using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.NewEntities;

public partial class Promotion
{
    public string PromoCode { get; set; } = null!;

    public decimal DiscountPercent { get; set; }

    public byte? MinDays { get; set; }

    public DateOnly ValidFrom { get; set; }

    public DateOnly ValidTo { get; set; }

    public int? UsageLimit { get; set; }

    public int? UsedCount { get; set; }
}
