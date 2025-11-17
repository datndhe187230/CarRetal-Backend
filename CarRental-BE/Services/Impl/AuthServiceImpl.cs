using CarRental_BE.Data;
using CarRental_BE.Exceptions;
using CarRental_BE.Helpers;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.NewEntities;
using CarRental_BE.Models.VO;
using CarRental_BE.Repositories;
using CloudinaryDotNet;
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
        private readonly IEmailService _emailService;
        private readonly IRedisService _redisService;

        public AuthServiceImpl(CarRentalContext carRentalContext, IConfiguration config, IAccountRepository accountRepository, IUserRepository userRepository, IEmailService emailService, IRedisService redisService)
        {
            _carRentalContext = carRentalContext;
            _config = config;
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _redisService = redisService;
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
                throw new UserNotFoundException(loginDto.Email);
            }
            //else if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, userAccount.Password))
            // else if (!String.Equals(loginDto.Password, userAccount.Password, StringComparison.Ordinal))
            else if (!PasswordHelper.VerifyPassword(loginDto.Password, userAccount.PasswordHash))
            {
                throw new UnauthorizedException("Invalid password.");
            }

            var fullName = await _userRepository.GetUserProfileFullNameByAccountId(userAccount.AccountId);

            var roleAccount = userAccount.Role;
            var idAccount = userAccount.AccountId;
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

        public async Task<LoginVO> LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal)
        {
            string accessToken = string.Empty;

            if (claimsPrincipal == null)
            {
                throw new ExternalLoginProviderException("Google", "ClaimsPrincipal is null");
            }
            var claims = claimsPrincipal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "unknown";

            if (email == null)
            {
                throw new ExternalLoginProviderException("Google", "Email is null");
            }

            var userAccount = await _accountRepository.getAccountByEmailWithRole(email);

            if (userAccount == null)
            {
                var newAccount = new Models.NewEntities.Account
                {
                    AccountId = Guid.NewGuid(),
                    Email = email,
                    PasswordHash = PasswordHelper.HashPassword(Guid.NewGuid().ToString()),
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    RoleId = 3
                };
                var newUserProfile = new UserProfile
                {
                    AccountId = newAccount.AccountId,
                    FullName = "Google User",
                    Account = newAccount
                };
                var newWallet = new Wallet
                {
                    AccountId = newAccount.AccountId,
                    BalanceCents = 0m,
                    LockedCents = 0m,
                    Account = newAccount,
                    UpdatedAt = DateTime.UtcNow
                };

                var created_account = await _accountRepository.CreateAccountAsync(newAccount, newUserProfile, newWallet);

                userAccount = created_account;

            }

            var fullName = "Google User";

            var roleAccount = userAccount.Role;
            var idAccount = userAccount.AccountId;
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var key = _config["Jwt:SecretKey"];
            var tokenValidityMins = _config.GetValue<int>("Jwt:TokenValidityMins");
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(JwtRegisteredClaimNames.Email, email),
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
            accessToken = token;

            return new LoginVO
            {
                Token = accessToken
            };
        }

        public async Task ResetPasswordAsync(ChangePasswordDTO changePasswordDto)
        {
            await _userRepository.ResetPassword(changePasswordDto);
        }

        public async Task SendEmailResetPasswordAsync(ForgotPasswordDTO forgotPasswordDto)
        {
            var account = await _accountRepository.getAccountByEmailWithRole(forgotPasswordDto.Email);
            if (account == null)
            {
                throw new UserNotFoundException(forgotPasswordDto.Email);
            }

            // 1. Generate reset token
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var key = _config["Jwt:SecretKey"];
            var tokenValidityMins = _config.GetValue<int>("Jwt:TokenValidityMins");
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);
            var jti = Guid.NewGuid().ToString();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim(JwtRegisteredClaimNames.Email, forgotPasswordDto.Email),
                        new(JwtRegisteredClaimNames.Jti, jti),

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

            // 2. Build the reset link
            var resetLink = $"http://localhost:3000/reset-password?token={token}";

            // 3. Email content
            var body = $"<p>Click the link below to reset your password:</p><a href='{resetLink}'>Reset Password</a>";

            // 4. Send the email
            await _emailService.SendEmailAsync(forgotPasswordDto.Email, "Password Reset Request", body);

            // 5. Save token_jti into Redis
            await _redisService.SaveTokenAsync("Forgot_Password_JTI", forgotPasswordDto.Email, jti, TimeSpan.FromMinutes(60));
        }
    }
}
