using CarRental_BE.Data;
using CarRental_BE.Models;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VOs.User;
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
        public async Task<ApiResponse<object>> GetUserProfile(Guid id)
        {
            try
            {
                var profile = await _userService.GetUserProfile(id);
                if (profile == null)
                {
                    return new ApiResponse<object>(404, "User profile not found", null);
                }

                return new ApiResponse<object>(200, "Success", profile);
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(500, "Server error", ex.Message);
            }
        }


        [HttpPut("profile/{id}")]
        public async Task<ApiResponse<object>> UpdateUserProfile(Guid id, [FromBody] UserUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return new ApiResponse<object>(400, "Validation failed", errors);
            }

            try
            {
                var result = await _userService.UpdateUserProfile(id, dto);
                if (result == null)
                {
                    return new ApiResponse<object>(404, "User not found", null);
                }

                return new ApiResponse<object>(200, "User profile updated successfully", result);
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(500, "Server error", ex.Message);
            }
        }




    }
}
