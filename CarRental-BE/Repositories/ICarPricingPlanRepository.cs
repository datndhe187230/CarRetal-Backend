using CarRental_BE.Models.NewEntities;

namespace CarRental_BE.Repositories
{
    public interface ICarPricingPlanRepository
    {
        Task<CarPricingPlan?> GetActivePricingPlanAsync(Guid carId);

        Task AddPricingPlanAsync(CarPricingPlan pricingPlan);
        Task UpdatePricingPlanAsync(CarPricingPlan pricingPlan);

        Task<Car?> GetCarByIdForUpdateAsync(Guid carId);

    }
}
