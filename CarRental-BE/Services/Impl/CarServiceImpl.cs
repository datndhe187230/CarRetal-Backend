using AutoMapper;
using CarRental_BE.Models.Common;
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

    public async Task<PaginationResponse<CarVO_ViewACar>> GetCarsByUserId(
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
   
}