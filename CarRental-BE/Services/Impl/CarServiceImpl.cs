using AutoMapper;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Repositories;
using CarRental_BE.Services;
using InvalidOperationException = CarRental_BE.Exceptions.InvalidOperationException;
using CarRental_BE.Models.VO.Car;
using CarRental_BE.Models.NewEntities;

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
        if (car == null) return null;
        return _mapper.Map<CarVO_Full>(car);
    }

    public async Task<PaginationResponse<CarVO_ViewACar>> GetCarsByAccountId(Guid accountId, PaginationRequest request)
    {
        var (cars, totalCount) = await _carRepository.GetAccountId(accountId, request.PageNumber, request.PageSize);
        var mapped = _mapper.Map<List<CarVO_ViewACar>>(cars);
        return new PaginationResponse<CarVO_ViewACar>(mapped, totalCount, request.PageSize, request.PageNumber);
    }

    public async Task<CarVO_CarDetail> GetCarDetailById(Guid carId)
    {
        var car = await _carRepository.GetByIdWithBookings(carId);
        if (car == null) return null;
        var carDetail = _mapper.Map<CarVO_CarDetail>(car);
        var completedBookings = car.Bookings?.Where(b => b.Status.Equals("completed", StringComparison.OrdinalIgnoreCase)).ToList() ?? new List<CarRental_BE.Models.NewEntities.Booking>();
        carDetail.NumberOfRides = completedBookings.Count;
        int totalRatingSum =0; int feedbackCount =0;
        foreach (var booking in completedBookings)
        {
            foreach (var review in booking.Reviews)
            {
                totalRatingSum += review.Rating;
                feedbackCount++;
            }
        }
        carDetail.TotalRating = feedbackCount;
        carDetail.Rating = feedbackCount >0 ? (int)Math.Round((double)totalRatingSum / feedbackCount) :0;
        return carDetail;
    }

    public async Task<PaginationResponse<CarSearchVO>> SearchCar(SearchDTO searchDTO, PaginationRequest requestPage)
    {
        var (cars, totalCount) = await _carRepository.SearchCar(searchDTO, requestPage.PageNumber, requestPage.PageSize);
        var mapped = _mapper.Map<List<CarSearchVO>>(cars);
        return new PaginationResponse<CarSearchVO>(mapped, requestPage.PageNumber, requestPage.PageSize, totalCount);
    }

    public async Task<CarVO_CarDetail> AddCar(AddCarDTO addCarDTO)
    {
        var car = await _carRepository.AddCar(addCarDTO);
        if (car == null) throw new InvalidOperationException("Failed to add car.");
        var detail = _mapper.Map<CarVO_CarDetail>(car);
        return detail;
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

    public Task<bool> CheckBookingAvailable(Guid carId, DateTime pickupDate, DateTime dropoffDate)
    {
        return _carRepository.CheckCarBookingStatus(carId, pickupDate, dropoffDate);
    }
}