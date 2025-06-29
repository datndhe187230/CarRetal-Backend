using CarRental_BE.Data;
using CarRental_BE.Exceptions;
using CarRental_BE.Models;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO.User;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
        public async Task<ApiResponse<object>> UpdateUserProfile(Guid id, [FromForm] UserUpdateDTO dto)
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
            catch (EmailExistException ex)
            {
                return new ApiResponse<object>(400, "Email already exists", ex.Message);
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(500, ex.Message, ex.Message);
            }
        }

        [HttpPost("change-password/{id}")]
        public async Task<ApiResponse<object>> ChangePassword(Guid id, [FromBody] ChangePasswordDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return new ApiResponse<object>(400, "Validation failed", errors);
            }

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return new ApiResponse<object>(400, "New password and confirmation don't match", null);
            }

            try
            {
                var result = await _userService.ChangePassword(id, dto);
                if (!result)
                {
                    return new ApiResponse<object>(400, "Password change failed. Check your current password.", null);
                }

                return new ApiResponse<object>(200, "Password changed successfully", null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(500, "Server error", ex.Message);
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ApiResponse<object>> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return new ApiResponse<object>(400, "Validation failed", errors);
            }

            if (dto.Password != dto.ConfirmPassword)
            {
                return new ApiResponse<object>(400, "Password and confirmation don't match", null);
            }

            try
            {
                var result = await _userService.Register(dto);
                if (!result)
                {
                    return new ApiResponse<object>(400, "Registration failed. Email may already exist.", null);
                }

                return new ApiResponse<object>(200, "Registration successful", null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(500, "Server error", ex.Message);
            }
        }


    }
}
