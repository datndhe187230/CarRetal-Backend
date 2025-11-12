using System;
using System.Collections.Generic;

namespace CarRental_BE.Models.NewEntities;

public partial class Booking
{
    public string BookingNumber { get; set; } = null!;

    public Guid CarId { get; set; }

    public Guid RenterAccountId { get; set; }

    public Guid? PricingPlanId { get; set; }

    public decimal? BasePriceSnapshotCents { get; set; }

    public decimal? DepositSnapshotCents { get; set; }

    public Guid? PickUpAddressId { get; set; }

    public Guid? DropOffAddressId { get; set; }

    public DateTime PickUpTime { get; set; }

    public DateTime DropOffTime { get; set; }

    public DateTime? ActualReturnTime { get; set; }

    public decimal? KmDriven { get; set; }

    public decimal? ExtraChargesCents { get; set; }

    public string Status { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BookingDriver> BookingDrivers { get; set; } = new List<BookingDriver>();

    public virtual Car Car { get; set; } = null!;

    public virtual Address? DropOffAddress { get; set; }

    public virtual Address? PickUpAddress { get; set; }

    public virtual CarPricingPlan? PricingPlan { get; set; }

    public virtual Account RenterAccount { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
