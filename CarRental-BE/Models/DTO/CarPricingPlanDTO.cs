using Microsoft.AspNetCore.Mvc;

namespace CarRental_BE.Models.DTO
{
    public class CarPricingPlanDTO : Controller
    {
        public long? BasePricePerDay { get; set; }
        public long? Deposit { get; set; }
    }
}
