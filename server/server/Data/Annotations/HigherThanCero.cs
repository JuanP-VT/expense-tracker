using System.ComponentModel.DataAnnotations;

namespace server.Data.Annotations
{
    public class HigherThanCero : ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult($"{validationContext.DisplayName} cannot be null");
            }

            if ((decimal)value <= 0)
            {
                return new ValidationResult($"{validationContext.DisplayName} must be greater than zero");
            }

            // This is the correct way to return a successful validation
            return ValidationResult.Success;
        }
    }
}