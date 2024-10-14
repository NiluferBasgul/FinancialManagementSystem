using FinancialManagementSystem.Core.Models.Request;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.API.Controllers
{
    /// <summary>
    /// Controller responsible for handling tax-related calculations.
    /// </summary>
    [ApiController]
    [Route("api/tax")]
    [ExcludeFromCodeCoverage]
    public class TaxController : ControllerBase
    {
        /// <summary>
        /// Calculates the total tax based on income, deductions, and filing status.
        /// </summary>
        /// <param name="request">The tax calculation request containing income, deductions, and filing status.</param>
        /// <returns>An <see cref="IActionResult"/> containing the calculated tax information if successful, or a bad request error if input is invalid.</returns>
        /// <response code="200">Returns the calculated tax amounts.</response>
        /// <response code="400">If the income is less than or equal to zero, a bad request error is returned.</response>
        /// <response code="500">If an internal error occurs, a server error is returned.</response>
        [HttpPost("calculate")]
        public IActionResult CalculateTax([FromBody] TaxCalculationRequest request)
        {
            try
            {
                if (request.Income <= 0)
                {
                    return BadRequest("Income must be greater than zero.");
                }

                decimal income = request.Income;
                decimal deductions = request.Deductions;
                string filingStatus = request.FilingStatus;

                decimal personalIncomeTax = 0.10m * income;
                decimal pensionDisabilityInsurance = 0.188m * income;
                decimal healthInsurance = 0.075m * income;
                decimal unemploymentInsurance = 0.012m * income;

                decimal totalTax = personalIncomeTax + pensionDisabilityInsurance + healthInsurance + unemploymentInsurance;

                var response = new TaxCalculationResponse
                {
                    TotalTax = totalTax,
                    PersonalIncomeTax = personalIncomeTax,
                    PensionDisabilityInsurance = pensionDisabilityInsurance,
                    HealthInsurance = healthInsurance,
                    UnemploymentInsurance = unemploymentInsurance
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
