using CarRental_BE.Data;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.VO.AdminManagement;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace CarRental_BE.Controllers;

[ApiController]
[Route("api/[Controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateUserStatus(Guid id, [FromBody] UserStatusUpdateRequest request)
    {
        if (request == null)
        {
            return BadRequest(new ApiResponse<object>(400, "Request body cannot be empty."));
        }

        try
        {
            var currentUserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            {
                return Unauthorized(new ApiResponse<object>(401, "Invalid user token."));
            }

            var updatedAccount = await _userService.UpdateUserStatusAsync(id, request, currentUserId);
            if (updatedAccount == null)
            {
                return NotFound(new ApiResponse<object>(404, "User not found."));
            }

            var responseData = new { Id = updatedAccount.Id, IsActive = updatedAccount.IsActive };
            return Ok(new ApiResponse<object>(200, "User status updated successfully.", responseData));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new ApiResponse<object>(403, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>(500, "An unexpected error occurred: " + ex.Message));
        }
    }
}