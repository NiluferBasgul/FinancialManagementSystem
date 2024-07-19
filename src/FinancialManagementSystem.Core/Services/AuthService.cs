using System;
using System.Threading.Tasks;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Entities;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using FinancialManagementSystem.Core.Models;
using AuthResult = FinancialManagementSystem.Core.Models.AuthResult;
using BCrypt.Net;
using Microsoft.Extensions.Logging;

namespace FinancialManagementSystem.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResult> LoginAsync(LoginModel model)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(model.Username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    _logger.LogWarning($"Failed login attempt for username: {model.Username}");
                    return new AuthResult { Succeeded = false, ErrorMessage = "Invalid username or password" };
                }

                var token = GenerateJwtToken(user);
                _logger.LogInformation($"User logged in: {user.Username}");
                return new AuthResult { Succeeded = true, Token = token };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during login for username: {model.Username}");
                throw;
            }
        }

        public async Task<AuthResult> RegisterAsync(RegisterModel model)
        {
            try
            {
                if (await _userRepository.GetByUsernameAsync(model.Username) != null)
                {
                    _logger.LogWarning($"Registration attempt with existing username: {model.Username}");
                    return new AuthResult { Succeeded = false, ErrorMessage = "Username already exists" };
                }

                if (await _userRepository.GetByEmailAsync(model.Email) != null)
                {
                    _logger.LogWarning($"Registration attempt with existing email: {model.Email}");
                    return new AuthResult { Succeeded = false, ErrorMessage = "Email already exists" };
                }

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
                };

                await _userRepository.AddAsync(user);

                var token = GenerateJwtToken(user);
                _logger.LogInformation($"New user registered: {user.Username}");
                return new AuthResult { Succeeded = true, Token = token };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during registration for username: {model.Username}");
                throw;
            }
        }

        public int? ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                _logger.LogInformation($"Token validated for user ID: {userId}");
                return userId;
            }
            catch
            {
                return null;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}