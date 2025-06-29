using AutoMapper;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
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

    public async Task<CarVO_CarDetail> GetCarDetailById(Guid carId)
    {
        var car = await _carRepository.GetByIdWithBookings(carId);

        if (car == null)
        {
            return null;
        }

        var carDetail = _mapper.Map<CarVO_CarDetail>(car);

        // Calculate number of completed rides
        var completedBookings = car.Bookings?
            .Where(b => b.Status != null && b.Status.Equals("completed", StringComparison.OrdinalIgnoreCase))
            .ToList() ?? new List<Booking>();

        carDetail.NumberOfRides = completedBookings.Count;

        // Calculate rating and total rating from feedback
        int totalRatingSum = 0;
        int feedbackCount = 0;

        foreach (var booking in completedBookings)
        {
            if (booking.Feedback != null && booking.Feedback.Rating.HasValue)
            {
                totalRatingSum += booking.Feedback.Rating.Value;
                feedbackCount++;
            }
        }

        carDetail.TotalRating = feedbackCount;
        carDetail.Rating = feedbackCount > 0 ? (int)Math.Round((double)totalRatingSum / feedbackCount) : 0;

        return carDetail;
    }

    public async Task<PaginationResponse<CarSearchVO>> SearchCar(SearchDTO searchDTO, PaginationRequest requestPage)
    {
        var pageNumber = requestPage.PageNumber;
        var pageSize = requestPage.PageSize;

        var (cars, totalCount) = await _carRepository.SearchCar(searchDTO, pageNumber, pageSize);

        var mappedCars = _mapper.Map<List<CarSearchVO>>(cars);

        return new PaginationResponse<CarSearchVO>(mappedCars, pageNumber, pageSize, totalCount);
    }

    public async Task<CarVO_CarDetail> AddCar(AddCarDTO addCarDTO)
    {
        var car = await _carRepository.AddCar(addCarDTO);

        if (car == null)
        {
            throw new InvalidOperationException("Failed to add car.");
        }

        var carDetail = _mapper.Map<CarVO_CarDetail>(car);

        return carDetail;
    }
    public async Task<Car?> UpdateCarEntity(Guid carId, CarUpdateDTO updatedCar)
    {
        var car = await _carRepository.GetCarById(carId);
        if (car == null) return null;

        _mapper.Map(updatedCar, car);
        return await _carRepository.UpdateCar(car);
    }

    public async Task<CarVO_Full?> GetCarVOById(Guid carId)
    {
        var car = await _carRepository.GetCarById(carId);
        return car == null ? null : _mapper.Map<CarVO_Full>(car);
    }
}