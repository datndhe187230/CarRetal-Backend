using CarRental_BE.Data;
using CarRental_BE.Models;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.VO;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CarRental_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginVO>>> Login([FromBody] LoginDTO dto)
        {
            var result = await _authService.LoginAsync(dto);
            var response = new ApiResponse<LoginVO>(
                status: 200,
                message: "Connection successful",
                data: result
            );
            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok("Logged out");
        }
    }
}
