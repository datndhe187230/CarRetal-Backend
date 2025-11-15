// Plan:
// - Create a new AutoMapper Profile named `CarPricingPlanMapper` in namespace `CarRental_BE.Models.Mapper`.
// - Reference the new-entity `CarPricingPlan` via alias `NewPlan`.
// - Configure mapping `CreateMap<NewPlan, CarVO_PricePlan>` with members:
//   - `Id` <- `PlanId`
//   - `BasePricePerDay` <- `(long)BasePricePerDayCents`
//   - `PricePerExtraKm` <- `PricePerExtraKmCents`
//   - `Deposit` <- `(long)DepositCents`
//   - `IsActive` <- `IsActive ?? false`
// - Keep style consistent with existing mappers.

using AutoMapper;
using CarRental_BE.Models.VO;
using CarRental_BE.Models.VO.Car;
using NewPlan = CarRental_BE.Models.NewEntities.CarPricingPlan;

namespace CarRental_BE.Models.Mapper
{
    public class CarPricingPlanMapper : Profile
    {
        public CarPricingPlanMapper()
        {
            CreateMap<NewPlan, CarVO_PricePlan>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.PlanId))
                .ForMember(d => d.BasePricePerDay, o => o.MapFrom(s => (long)s.BasePricePerDayCents))
                .ForMember(d => d.PricePerExtraKm, o => o.MapFrom(s => s.PricePerExtraKmCents))
                .ForMember(d => d.Deposit, o => o.MapFrom(s => (long)s.DepositCents))
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.IsActive ?? false));
        }
    }
}