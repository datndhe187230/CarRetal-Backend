using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;

namespace CarRental_BE.Repositories
{
    public interface ICarRepository
    {
        Task<(List<Car> cars, int totalCount)> GetAccountId(
            Guid accountId, 
            int pageNumber, 
            int pageSize);

        Task<Car?> GetByIdWithBookings(Guid carId);
        Task<Car?> GetByIdAsync(Guid id);


        Task<(List<Car> cars, int totalCount)> SearchCar(SearchDTO searchDTO, int pageNumber, int pageSize);

        Task<Car?> AddCar(AddCarDTO addCarDTO);

        Task<List<CarSummaryDTO>> GetAllWithFeedback();
        Task<(List<Car> cars, int totalCount)> GetAllUnverifiedCarsAsync(
        int pageNumber,
        int pageSize,
        CarFilterDTO? filters = null); // Thêm tham số filters

        Task VerifyCarInfo(Guid carId);
        Task<(List<Car> cars, int totalCount)> GetAccountCarsFilteredAsync(Guid accountId, int pageNumber, int pageSize, CarFilterDTO filters);

    }
}
