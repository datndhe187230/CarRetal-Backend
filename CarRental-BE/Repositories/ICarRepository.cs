using CarRental_BE.Models.Entities;

namespace CarRental_BE.Repositories
{
    public interface ICarRepository
    {
        Task<(List<Car> cars, int totalCount)> GetAccountId(
            Guid accountId, 
            int pageNumber, 
            int pageSize);


        Task<Car?> GetCarById(Guid carId);

        Task<Car?> UpdateCar(Car car);
    }


}
