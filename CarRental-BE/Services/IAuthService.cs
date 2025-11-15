using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO;
using System.Security.Claims;

namespace CarRental_BE.Services
{
    public interface IAuthService
    {
        Task<LoginVO> LoginAsync(LoginDTO loginDto);

        Task SendEmailResetPasswordAsync(ForgotPasswordDTO forgotPasswordDto);

        Task ResetPasswordAsync(ChangePasswordDTO changePasswordDto);

        Task<LoginVO> LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal);

    }
}
