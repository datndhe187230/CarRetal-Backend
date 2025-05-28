using CarRental_BE.Data;
using CarRental_BE.Models;
using CarRental_BE.Models.VO.User;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRental_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/User/profile/{id}
        [HttpGet("profile/{id}")]
        public async Task<ActionResult<ApiResponse<UserProfileVO>>> GetUserProfile(Guid id)
        {
            try
            {
                var profile = await _userService.GetUserProfile(id);
                if (profile == null)
                {
                    return NotFound(new ApiResponse<string>(404, "User profile not found", null));
                }

                return Ok(new ApiResponse<UserProfileVO>(200, "Success", profile));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(500, "Server error", ex.Message));
            }
        }
    }
}
