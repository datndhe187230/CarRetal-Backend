using AutoMapper;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.Car;
using CarRental_BE.Repositories;
using CarRental_BE.Services;

public class CarServiceImpl : ICarService
{
    private readonly ICarRepository _carRepository;
    private readonly IMapper _mapper;

    public CarServiceImpl(ICarRepository carRepository, IMapper mapper)
    {
        _carRepository = carRepository;
        _mapper = mapper;
    }

    public async Task<CarVO_Full?> GetCarById(Guid carId)
    {
        var car = await _carRepository.GetCarById(carId);
        if (car == null)
        {
            return null;
        }
        return _mapper.Map<CarVO_Full>(car);
    }

    public async Task<PaginationResponse<CarVO_ViewACar>> GetCarsByAccountId(
        Guid accountId, 
        PaginationRequest request)
    {
        var pageNumber = request.PageNumber;
        var pageSize = request.PageSize;

        var (cars, totalCount) = await _carRepository.GetAccountId
            (accountId, pageNumber, pageSize);

        var mapperCars = _mapper.Map<List<CarVO_ViewACar>>(cars);

        return new PaginationResponse<CarVO_ViewACar>(mapperCars, totalCount, pageSize, pageNumber);
        
    }

    public async Task<CarVO_Full?> UpdateCar(Guid carId, CarUpdateDTO updatedCar)
    {
        var car = await _carRepository.GetCarById(carId);
        if (car == null)
        {
            return null;
        }
        _mapper.Map(updatedCar, car);
        var updatedCarEntity = await _carRepository.UpdateCar(car);
        if (updatedCarEntity == null)
        {
            return null;
        }
        return _mapper.Map<CarVO_Full>(updatedCarEntity);
    }


}