using CarRental_BE.Data;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.VO;
using CarRental_BE.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarRental_BE.Services.Impl
{
    public class AuthServiceImpl : IAuthService
    {
        private readonly CarRentalContext _carRentalContext;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public AuthServiceImpl(CarRentalContext carRentalContext, IConfiguration config, IAccountRepository accountRepository, IUserRepository userRepository)
        {
            _carRentalContext = carRentalContext;
            _config = config;
            _accountRepository = accountRepository;
            _userRepository = userRepository;
        }

        public async Task<LoginVO> LoginAsync(LoginDTO loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                throw new ArgumentException("Email and password must be provided.");
            }

            var userAccount = await _accountRepository.getAccountByEmailWithRole(loginDto.Email);

            if (userAccount == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            else if (!string.Equals(loginDto.Password, userAccount.Password))
            {
                throw new UnauthorizedAccessException("Invalid password.");
            }

            var fullName = await _userRepository.GetUserProfileFullNameByAccountId(userAccount.Id);

            var roleAccount = userAccount.Role;
            var idAccount = userAccount.Id;
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var key = _config["Jwt:SecretKey"];
            var tokenValidityMins = _config.GetValue<int>("Jwt:TokenValidityMins");
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim(JwtRegisteredClaimNames.Email, loginDto.Email),
                        new Claim(ClaimTypes.Role, roleAccount.Name!),
                        new Claim("id", idAccount.ToString()!),
                        new Claim("fullname", fullName ?? string.Empty)

                    }),
                Expires = tokenExpiryTimeStamp,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return new LoginVO
            {
                Token = token
            };
        }
    }
}
