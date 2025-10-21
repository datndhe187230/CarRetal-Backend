using CarRental_BE.Data;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.VO.Car;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Authorization;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarController : ControllerBase
{
    private readonly CarRentalContext _context;
    private readonly ICarService _carService;

    //Dependency Injection
    public CarController(CarRentalContext context, ICarService carService)
    {

        _context = context;
        _carService = carService;
    }


    //Dat
    [HttpGet]
    [Route("All")]
    public async Task<ActionResult<ApiResponse<List<Car>>>> GetAllCar()
    {
        try
        {
            // Fix: Replace AllAsync with ToListAsync to fetch all cars
            List<Car> anyCar = await _context.Cars.ToListAsync();
            var response = new ApiResponse<List<Car>>(
                status: 200,
                message: "Connection successful",
                data: anyCar
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new ApiResponse<string>(
                status: 500,
                message: "Connection failed",
                data: $"Connection failed: {ex.Message}"
            );
            return StatusCode(500, errorResponse);
        }
    }

    [HttpGet("test-connection")]
    
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            var anyCar = await _context.Cars.FirstOrDefaultAsync();
            return Ok(anyCar != null ? "Connected and data exists." : "Connected but no data.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Connection failed: {ex.Message}");
        }
    }

    //Hung
    [HttpGet("{accountId}/paginated")]
    [Authorize(Roles = "car_owner")]
    public async Task<ApiResponse<PaginationResponse<CarVO_ViewACar>>> GetCarsByAccountId(Guid accountId,
        [FromQuery] int PageNumber = 1,
        [FromQuery] int PageSize = 10)
    {
        try
        {
            var request = new PaginationRequest
            {
                PageNumber = PageNumber,
                PageSize = PageSize
            };

            var result = await _carService.GetCarsByAccountId(accountId, request);

            var response = new ApiResponse<PaginationResponse<CarVO_ViewACar>>(
                status: 200,
                message: "Connection successful",
                data: result
                );
            return response;
        }
        catch (Exception ex)
        {
            return new ApiResponse<PaginationResponse<CarVO_ViewACar>>(
                status: 500,
                message: "Connection failed",
                data: null
           );

        }
    }

    [HttpPatch("edit-car/{carId}")]
    [Authorize(Roles = "car_owner")]
    public async Task<ApiResponse<CarVO_Full>> UpdateCar(Guid carId, [FromBody] CarUpdateDTO updateDto)
    {
        try
        {
            var updatedCar = await _carService.UpdateCarEntity(carId, updateDto);
            if (updatedCar == null)
            {
                return new ApiResponse<CarVO_Full>(404, "Car not found", null);
            }

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

            if (carDetail == null)
            {
                return new ApiResponse<CarVO_CarDetail>(
                    status: 404,
                    message: "Car not found",
                    data: null);
            }

            return new ApiResponse<CarVO_CarDetail>(
                status: 200,
                message: "Car details retrieved successfully",
                data: carDetail);
        }
        catch (Exception ex)
        {
            return new ApiResponse<CarVO_CarDetail>(
                status: 500,
                message: $"Error retrieving car details: {ex.Message}",
                data: null);
        }
    }

    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<ApiResponse<PaginationResponse<CarSearchVO>>> SearchCar([FromQuery] SearchDTO searchDTO, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var requestPage = new PaginationRequest
        {
            PageNumber = page,
            PageSize = pageSize
        };

        PaginationResponse<CarSearchVO> list = await _carService.SearchCar(searchDTO, requestPage);

        var response = new ApiResponse<PaginationResponse<CarSearchVO>>(
            status: 200,
            message: "Search functionality is not implemented yet",
            data: list
        );

        return response;
    }

    [HttpPost("add")]
    [Authorize(Roles = "admin, car_owner")]
    public async Task<ApiResponse<CarVO_CarDetail>> AddCar([FromForm] AddCarDTO addCarDTO)
    {
            var newCar = await _carService.AddCar(addCarDTO);
            
            return new ApiResponse<CarVO_CarDetail>(
                status: 201,
                message: "Car added successfully",
                data: newCar);
        
    }

}
