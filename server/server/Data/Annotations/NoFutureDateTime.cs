using System.ComponentModel.DataAnnotations;

namespace server.Data.Annotations
{
    public class NoFutureDateTime : ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult($"{validationContext.DisplayName} cannot be null");
            }

            if ((DateTime)value > DateTime.UtcNow)
            {
                return new ValidationResult($"{validationContext.DisplayName} cannot be a future date");
            }

            // This is the correct way to return a successful validation
            return ValidationResult.Success;
        }
    }
}