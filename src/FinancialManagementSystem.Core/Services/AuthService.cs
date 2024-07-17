using System;
using System.Threading.Tasks;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Entities;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace FinancialManagementSystem.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResult> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || !VerifyPasswordHash(password, user.PasswordHash))
            {
                return new AuthResult { Succeeded = false, Errors = new[] { "Invalid username or password" } };
            }

            var token = GenerateJwtToken(user);
            return new AuthResult { Succeeded = true, Token = token };
        }

        public async Task<AuthResult> RegisterAsync(string username, string email, string password)
        {
            if (await _userRepository.GetByUsernameAsync(username) != null)
            {
                return new AuthResult { Succeeded = false, Errors = new[] { "Username already exists" } };
            }

            if (await _userRepository.GetByEmailAsync(email) != null)
            {
                return new AuthResult { Succeeded = false, Errors = new[] { "Email already exists" } };
            }

            var passwordHash = CreatePasswordHash(password);
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            return new AuthResult { Succeeded = true };
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            // Implement password verification logic
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        private string CreatePasswordHash(string password)
        {
            // Implement password hashing logic
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}