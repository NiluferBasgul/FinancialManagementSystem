using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinancialManagementSystem.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing user accounts.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor that injects the user service.
        /// </summary>
        /// <param name="userService">User service interface.</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves the details of a specific user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user details if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// Updates the details of an existing user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="model">UserUpdateModel containing updated user details.</param>
        /// <returns>The updated user details if successful, otherwise BadRequest or NotFound.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateModel model)
        {
            if (id != model.Id)
                return BadRequest("Id mismatch");

            var result = await _userService.UpdateUserAsync(model);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Deletes an existing user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>NoContent if successful, otherwise NotFound.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Retrieves the details of the currently authenticated user.
        /// </summary>
        /// <returns>The current user's details, or Unauthorized if not authenticated.</returns>
        [Authorize]
        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);
            var user = await _userService.GetUserAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            });
        }
    }
}
