using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.Entities;

public partial class CarPricingPlan
{
    public Guid PlanId { get; set; }

    public Guid CarId { get; set; }

    public string? Name { get; set; }

    public decimal BasePricePerDayCents { get; set; }

    public decimal DepositCents { get; set; }

    public int? KmIncludedDaily { get; set; }

    public int? PricePerExtraKmCents { get; set; }

    public byte? MinDays { get; set; }

    public byte? MaxDays { get; set; }

    public bool? IsWeekendOnly { get; set; }

    public decimal? DiscountPercent { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Car Car { get; set; } = null!;
}
