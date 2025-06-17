using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.Car;

namespace CarRental_BE.Services
{
    public interface ICarService
    {
        Task<PaginationResponse<CarVO_ViewACar>> GetCarsByUserId(Guid accountId, PaginationRequest request);
        Task<CarVO_CarDetail> GetCarDetailById(Guid carId);
        Task<PaginationResponse<CarSearchVO>> SearchCar(SearchDTO searchDTO, PaginationRequest requestPage);
        Task<CarVO_CarDetail> AddCar(AddCarDTO addCarDTO);

    }

}
