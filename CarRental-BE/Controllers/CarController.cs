using CarRental_BE.Models.Common; // ensure ApiResponse
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.Car;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarEntity = CarRental_BE.Models.NewEntities.Car;
using CarRental_BE.Models.NewEntities;
using CarRental_BE.Data;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarController : ControllerBase
{
    private readonly ICarService _carService;

    public CarController( ICarService carService)
    {
        _carService = carService;
    }

    //[HttpGet]
    //[Route("All")]
    //public async Task<ActionResult<ApiResponse<List<CarEntity>>>> GetAllCar()
    //{
    //    try
    //    {
    //        var anyCar = _carService.Car;
    //        var response = new ApiResponse<List<CarEntity>>(200, "Connection successful", anyCar);
    //        return Ok(response);
    //    }
    //    catch (Exception ex)
    //    {
    //        var error = new ApiResponse<string>(500, "Connection failed", $"Connection failed: {ex.Message}");
    //        return StatusCode(500, error);
    //    }
    //}


    [HttpGet("{accountId}/paginated")]
    [Authorize(Roles = "car_owner")]
    public async Task<ApiResponse<PaginationResponse<CarVO_ViewACar>>> GetCarsByAccountId(Guid accountId, [FromQuery] int PageNumber =1, [FromQuery] int PageSize =10)
    {
        try
        {
            var request = new PaginationRequest { PageNumber = PageNumber, PageSize = PageSize };
            var result = await _carService.GetCarsByAccountId(accountId, request);
            return new ApiResponse<PaginationResponse<CarVO_ViewACar>>(200, "Connection successful", result);
        }
        catch
        {
            return new ApiResponse<PaginationResponse<CarVO_ViewACar>>(500, "Connection failed", null);
        }
    }

    [HttpPatch("edit-car/{carId}")]
    [Authorize(Roles = "car_owner")]
    public async Task<ApiResponse<CarVO_Full>> UpdateCar(Guid carId, [FromBody] CarUpdateDTO updateDto)
    {
        try
        {
            var updatedCar = await _carService.UpdateCarEntity(carId, updateDto);
            if (updatedCar == null) return new ApiResponse<CarVO_Full>(404, "Car not found", null);
            var result = await _carService.GetCarVOById(carId);
            return new ApiResponse<CarVO_Full>(200, "Update Success", result);
        }
        catch (Exception ex)
        {
            return new ApiResponse<CarVO_Full>(500, $"Server error: {ex.Message}", null);
        }
    }

    [HttpGet("{carId}/detail")]
    [AllowAnonymous]
    public async Task<ApiResponse<CarVO_CarDetail>> GetCarDetail(Guid carId)
    {
        try
        {
            var carDetail = await _carService.GetCarDetailById(carId);
            if (carDetail == null) return new ApiResponse<CarVO_CarDetail>(404, "Car not found", null);
            return new ApiResponse<CarVO_CarDetail>(200, "Car details retrieved successfully", carDetail);
        }
        catch (Exception ex)
        {
            return new ApiResponse<CarVO_CarDetail>(500, $"Error retrieving car details: {ex.Message}", null);
        }
    }

    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<ApiResponse<PaginationResponse<CarSearchVO>>> SearchCar([FromQuery] SearchDTO searchDTO, [FromQuery] int page =1, [FromQuery] int pageSize =10)
    {
        var requestPage = new PaginationRequest { PageNumber = page, PageSize = pageSize };
        var list = await _carService.SearchCar(searchDTO, requestPage);
        return new ApiResponse<PaginationResponse<CarSearchVO>>(200, "Successful", list);
    }

    [HttpPost("add")]
    [Authorize(Roles = "admin, car_owner")]
    public async Task<ApiResponse<CarVO_CarDetail>> AddCar([FromForm] AddCarDTO addCarDTO)
    {
        var newCar = await _carService.AddCar(addCarDTO);
        return new ApiResponse<CarVO_CarDetail>(201, "Car added successfully", newCar);
    }
}
