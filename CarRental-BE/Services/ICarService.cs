using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.Car;

namespace CarRental_BE.Services
{
    public interface ICarService
    {
        Task<CarVO_CarDetail> GetCarDetailById(Guid carId);

        Task<PaginationResponse<CarVO_ViewACar>> GetCarsByAccountId(Guid accountId, PaginationRequest request);

        Task<CarVO_Full?> GetCarById(Guid carId);

        Task<CarVO_Full?> UpdateCar(Guid carId, CarUpdateDTO updatedCar); 
    }

}
