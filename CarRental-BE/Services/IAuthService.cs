using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO;

namespace CarRental_BE.Services
{
    public interface IAuthService
    {
        Task<LoginVO> LoginAsync(LoginDTO loginDto);
    }
}
