using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FinancialManagementSystem.API.Tests.Controllers
{
    // FinancialManagementSystem.API/Controllers/GoalController.cs
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GoalController : ControllerBase
    {
        private readonly IGoalService _goalService;

        public GoalController(IGoalService goalService)
        {
            _goalService = goalService;
        }

        [HttpGet]
        public async Task<IActionResult> GetGoals()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var goals = await _goalService.GetGoalsAsync(userId);
            return Ok(goals);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGoal(int id)
        {
            var goal = await _goalService.GetGoalAsync(id);
            if (goal == null)
                return NotFound();

            return Ok(goal);
        }

        [HttpPost]
        public async Task<IActionResult> AddGoal(GoalModel model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            model.UserId = userId;
            var result = await _goalService.AddGoalAsync(model);
            return CreatedAtAction(nameof(GetGoal), new { id = result.Id }, result);
        }

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            await _goalService.DeleteGoalAsync(id);
            return NoContent();
        }

        [HttpGet("{id}/recommendations")]
        public async Task<IActionResult> GetRecommendations(int id)
        {
            var recommendations = await _goalService.GetRecommendationsAsync(id);
            return Ok(recommendations);
        }
    }
}
