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
        private readonly IEmailService _emailService;

        public AuthController(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginVO>>> Login([FromBody] LoginDTO dto)
        {
            var result = await _authService.LoginAsync(dto);

            Response.Cookies.Append("Access_Token", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            var response = new ApiResponse<LoginVO>(
                status: 200,
                message: "Connection successful",
                data: result
            );
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("send-email-forgot-password")]
        public async Task<IActionResult> SendEmail([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            await _authService.SendEmailResetPasswordAsync(forgotPasswordDTO);

            return Ok(new ApiResponse<string>(
                 status: 200,
                 message: "Email sent successfully",
                 data: null
            ));
        }

        [Authorize]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            await _authService.ResetPasswordAsync(changePasswordDTO);
            return Ok(new ApiResponse<string>(
                status: 200,
                message: "Password reset successfully",
                data: null
            ));
        }


    }
}