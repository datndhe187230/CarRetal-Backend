using AutoMapper;
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

    public async Task<List<CarVO_ViewACar>> GetCarsByUserId(Guid accountId)
    {
        var cars = await _carRepository.GetAccountId(accountId);

        if (cars == null || !cars.Any())
            return new List<CarVO_ViewACar>();

        return _mapper.Map<List<CarVO_ViewACar>>(cars);
    }

}