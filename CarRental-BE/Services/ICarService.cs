using CarRental_BE.Models.VO.Car;

namespace CarRental_BE.Services
{
    public interface ICarService
    {
        Task<List<CarVO_ViewACar>> GetCarsByUserId(Guid accountId);
    }

}
