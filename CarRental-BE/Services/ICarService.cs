using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.Car;
using CarRental_BE.Models.NewEntities;

namespace CarRental_BE.Services
{
    public interface ICarService
    {
        Task<CarVO_CarDetail> GetCarDetailById(Guid carId);
        Task<PaginationResponse<CarSearchVO>> SearchCar(SearchDTO searchDTO, PaginationRequest requestPage);
        Task<CarVO_CarDetail> AddCar(AddCarDTO addCarDTO);

        Task<PaginationResponse<CarVO_ViewACar>> GetCarsByAccountId(Guid accountId, PaginationRequest request);

        Task<CarRental_BE.Models.NewEntities.Car?> UpdateCarEntity(Guid carId, CarUpdateDTO updatedCar);
        Task<CarVO_Full?> GetCarVOById(Guid carId);
        Task<bool> CheckBookingAvailable(Guid carId, DateTime pickupDate, DateTime dropoffDate);
    }

}
