using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRental_BE.Models;
using CarRental_BE;
using EmployeeAdminPortal.Models;

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
    public async Task<ActionResult<ApiResponse<string>>> GetAllCar()
    {
        try
        {
            var anyCar = await _context.Cars.FirstOrDefaultAsync();

            if (anyCar != null)
            {
                return Ok(new ApiResponse<string>(200, "Connected and data exists.", "Connected"));
            }
            else
            {
                return Ok(new ApiResponse<string>(200, "Connected but no data.", "No data"));
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>(500, $"Connection failed: {ex.Message}", null));
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
