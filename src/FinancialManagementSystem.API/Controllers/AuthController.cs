using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagementSystem.API.Controllers
{
    /// <summary>
    /// Controller responsible for handling user authentication.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">Service to handle authentication logic.</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Logs a user in and returns a JWT token if successful.
        /// </summary>
        /// <param name="model">Login model containing the username and password.</param>
        /// <returns>JWT token if successful, otherwise Unauthorized.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _authService.LoginAsync(model);
            if (result.Succeeded)
            {
                return Ok(new { Token = result.Token });
            }
            return Unauthorized(result.ErrorMessage);
        }

        /// <summary>
        /// Registers a new user and returns a JWT token if successful.
        /// </summary>
        /// <param name="model">Registration model containing user details.</param>
        /// <returns>JWT token if successful, otherwise BadRequest.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _authService.RegisterAsync(model);
            if (result.Succeeded)
            {
                return Ok(new { Token = result.Token });
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
