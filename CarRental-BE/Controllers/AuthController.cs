using CarRental_BE.Data;
using CarRental_BE.Models;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.VO;
using CarRental_BE.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly LinkGenerator _linkGenerator;
        private readonly SignInManager<Account> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(IAuthService authService, IEmailService emailService, SignInManager<Account> signInManager, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _emailService = emailService;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
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

        [AllowAnonymous]
        [HttpGet("login/google")]
        public async Task<IActionResult> GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleCallback))
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
           
            Console.WriteLine("Google Callback Invoked");

            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded || result?.Principal == null)
            {
                return BadRequest("DAHELLLL");

            }

            var token = await _authService.LoginWithGoogleAsync(result.Principal);

            Response.Cookies.Append("Access_Token", token.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            return Redirect("http://localhost:3000");
        }
    }
}