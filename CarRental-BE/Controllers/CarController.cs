using CarRental_BE.Data;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.VO.Car;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "customer")]
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

    [HttpGet("edit-car/{carId}")]
    public async Task<ApiResponse<object>> UpdateCar(Guid carId)
    {
        try
        {
            var car = await _carService.GetCarById(carId);
            if (car == null)
            {
                return new ApiResponse<object>(404, "Car not found", null);
            }
            return new ApiResponse<object>(200, "Update Success", car);
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>(500, "Server error", ex.Message);
        }


    }

}
