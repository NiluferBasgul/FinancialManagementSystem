using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Models
{
    [ExcludeFromCodeCoverage]
    public class GoalModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Goal name is required")]
        [StringLength(100, ErrorMessage = "Goal name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Target amount must be greater than zero")]
        public decimal TargetAmount { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Target date is required")]
        [DateGreaterThan(nameof(StartDate), ErrorMessage = "Target date must be later than start date")]
        public DateTime TargetDate { get; set; }

        public bool IsCompleted { get; set; }

        public decimal CurrentAmount { get; set; }

        public decimal ProgressPercentage => TargetAmount > 0 ? (CurrentAmount / TargetAmount) * 100 : 0;
    }

    [ExcludeFromCodeCoverage]
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTime?)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (DateTime?)property.GetValue(validationContext.ObjectInstance);

            if (currentValue <= comparisonValue)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
