using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.Car;

namespace CarRental_BE.Services
{
    public interface ICarService
    {
        Task<PaginationResponse<CarVO_ViewACar>> GetCarsByUserId(Guid accountId, PaginationRequest request);
        Task<CarVO_CarDetail> GetCarDetailById(Guid carId);
        Task<CarVO_CarDetail> AddCar(AddCarDTO addCarDTO);

    }

}
