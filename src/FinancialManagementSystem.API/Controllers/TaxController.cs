using FinancialManagementSystem.Core.Models;
using FinancialManagementSystem.Core.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/tax")]
    public class TaxController : ControllerBase
    {
        // POST: /api/tax/calculate
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

                // Apply North Macedonian tax rules
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