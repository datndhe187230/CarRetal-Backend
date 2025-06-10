using CarRental_BE.Data;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.VO.Car;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CarController : ControllerBase
{
    private readonly CarRentalContext _context;
    private readonly ICarService _carService;
    private readonly ICloudinaryService _cloudinaryService;

    //Dependency Injection
    public CarController(CarRentalContext context, ICarService carService, ICloudinaryService cloudinaryService)
    {

        _context = context;
        _carService = carService;
        _cloudinaryService = cloudinaryService;
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

    [HttpGet("test-upload")]
    public async Task<IActionResult> TestUploadConnection()
    {
        try
        {
            var result = await _cloudinaryService.TestUpload();
            var response = new ApiResponse<string>(
                status: 200,
                message: "Upload test successful",
                data: result
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new ApiResponse<string>(
                status: 500,
                message: "Upload test failed",
                data: $"Error: {ex.Message}"
            );
            return StatusCode(500, errorResponse);
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

            var result = await _carService.GetCarsByUserId(accountId, request);

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

    [HttpGet("{carId}/detail")]
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

}
