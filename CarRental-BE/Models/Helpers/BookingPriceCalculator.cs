using System;
using CarRental_BE.Models.NewEntities;

namespace CarRental_BE.Models.Helpers
{
    public static class BookingPriceCalculator
    {
        public static decimal CalculateTotalCents(Booking booking)
        {
            var plan = booking.PricingPlan;
            if (plan == null)
                throw new InvalidOperationException("Booking must have an associated PricingPlan.");

            // Prefer snapshots when present to ensure consistency
            var basePricePerDay = plan.BasePricePerDayCents;
            var deposit = booking.DepositSnapshotCents ?? plan.DepositCents;

            //1. Rental duration in whole days (ceil on hours/24)
            var duration = booking.DropOffTime - booking.PickUpTime;
            var totalDays = (int)Math.Ceiling(duration.TotalHours / 24d);
            if (totalDays < 1) totalDays = 1;

            //2. Base price
            var basePrice = basePricePerDay * totalDays;

            //3. Extra km fee (only when return completed and data available)
            decimal extraKmFee = 0m;
            if (booking.KmDriven.HasValue && plan.KmIncludedDaily.HasValue && plan.PricePerExtraKmCents.HasValue)
            {
                var allowedKm = plan.KmIncludedDaily.Value * totalDays;
                var extraKm = Math.Max(0m, booking.KmDriven.Value - allowedKm);
                extraKmFee = extraKm * plan.PricePerExtraKmCents.Value;
            }

            //4. Discount
            decimal discount = 0m;
            if (plan.DiscountPercent.HasValue && plan.DiscountPercent.Value > 0)
            {
                discount = basePrice * (plan.DiscountPercent.Value / 100m);
            }

            //5. Extra charges (optional)
            var extraCharges = booking.ExtraChargesCents ?? 0m;

            //6. Total
            var total = basePrice + extraKmFee + deposit - discount + extraCharges;
            return Math.Max(total, 0m);
        }

        public static int CalculateTotalDays(DateTime pickUp, DateTime dropOff)
        {
            var totalDays = (int)Math.Ceiling((dropOff - pickUp).TotalHours / 24d);
            return totalDays < 1 ? 1 : totalDays;
        }
    }
}
