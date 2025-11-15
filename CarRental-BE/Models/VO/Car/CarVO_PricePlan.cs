using System;

namespace CarRental_BE.Models.VO.Car
{
    public record CarVO_PricePlan
    {
        public Guid Id { get; set; }
        public long BasePricePerDay { get; set; }
        public int? PricePerExtraKm { get; set; }
        public long Deposit { get; set; }
        public bool IsActive { get; set; }
    }
}