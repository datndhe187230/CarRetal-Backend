using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental_BE.Data;
using CarRental_BE.Models.Entities;

[ApiController]
[Route("api/[controller]")]
public class CarController : ControllerBase
{
    private readonly CarRentalContext _context;

    public CarController(CarRentalContext context)
    {
        _context = context;
    }

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
}
