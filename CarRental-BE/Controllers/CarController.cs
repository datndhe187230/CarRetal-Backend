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
            var anyCar = _context.Cars.FirstOrDefaultAsync();
            Console.WriteLine(anyCar != null ? "Connected and data exists." : "Connected but no data.");
            return anyCar != null ? "Connected and data exists." : "Connected but no data.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection failed: {ex.Message}");

            return $"Connection failed: {ex.Message}";
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
