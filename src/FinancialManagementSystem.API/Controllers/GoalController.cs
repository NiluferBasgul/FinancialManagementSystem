using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FinancialManagementSystem.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing user goals.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GoalController : ControllerBase
    {
        private readonly IGoalService _goalService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoalController"/> class.
        /// </summary>
        /// <param name="goalService">Service for managing goals.</param>
        public GoalController(IGoalService goalService)
        {
            _goalService = goalService;
        }

        /// <summary>
        /// Retrieves the goals for the authenticated user.
        /// </summary>
        /// <returns>A list of goals for the user.</returns>
        [HttpGet]
        public async Task<IActionResult> GetGoals()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var goals = await _goalService.GetGoalsAsync(userId);
            return Ok(goals);
        }

        /// <summary>
        /// Retrieves a specific goal by its ID.
        /// </summary>
        /// <param name="id">The ID of the goal.</param>
        /// <returns>The requested goal if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGoal(int id)
        {
            var goal = await _goalService.GetGoalAsync(id);
            if (goal == null)
                return NotFound();

            return Ok(goal);
        }

        /// <summary>
        /// Adds a new goal for the authenticated user.
        /// </summary>
        /// <param name="model">The goal model containing goal details.</param>
        /// <returns>The created goal if successful, otherwise BadRequest.</returns>
        [HttpPost]
        public async Task<IActionResult> AddGoal(GoalModel model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            model.UserId = userId;
            var result = await _goalService.AddGoalAsync(model);
            return CreatedAtAction(nameof(GetGoal), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing goal for the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the goal to update.</param>
        /// <param name="model">The updated goal model.</param>
        /// <returns>NoContent if successful, otherwise BadRequest or NotFound.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGoal(int id, GoalModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var result = await _goalService.UpdateGoalAsync(model);
            if (result == null)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Deletes a goal by its ID.
        /// </summary>
        /// <param name="id">The ID of the goal to delete.</param>
        /// <returns>NoContent if successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            await _goalService.DeleteGoalAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Retrieves personalized recommendations for a specific goal by its ID.
        /// </summary>
        /// <param name="id">The ID of the goal.</param>
        /// <returns>A list of personalized recommendations.</returns>
        [HttpGet("{id}/recommendations")]
        public async Task<IActionResult> GetRecommendations(int id)
        {
            var recommendations = await _goalService.GetRecommendationsAsync(id);
            return Ok(recommendations);
        }
    }
}
