using CarRental_BE.Data;
using CarRental_BE.Models.NewEntities;
using CarRental_BE.Models.VO.AdminManagement;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;

namespace CarRental_BE.Controllers;

[Route("api/[Controller]")] 
[ApiController]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IUserService userService, ILogger<AdminController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [Authorize(Roles ="admin")]
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateUserStatus(Guid id, [FromBody] UserStatusUpdateRequest request)
    {
        if (request == null)
        {
            return BadRequest(new ApiResponse<object>(400, "Request body cannot be empty or invalid."));
        }

        try
        {
            var currentUserId = Guid.Parse("9A2EB519-7054-4A1A-BAED-A33DCA077C37");

            var updatedAccount = await _userService.UpdateUserStatusAsync(id, request, currentUserId);
            if (updatedAccount == null)
            {
                return NotFound(new ApiResponse<object>(404, "User not found."));
            }

            var responseData = new { Id = updatedAccount.AccountId, IsActive = updatedAccount.IsActive };
            return Ok(new ApiResponse<object>(200, "User status updated successfully.", responseData));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user status for user ID {UserId}: {Message}", id, ex.Message);
            return StatusCode(500, new ApiResponse<object>(500, "An unexpected error occurred."));
        }
    }

}