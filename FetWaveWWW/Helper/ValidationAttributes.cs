using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MeetWave.Helper
{
    public class BeforeEndDateAttribute : ValidationAttribute
    {
        public string EndDatePropertyName { get; set; }


        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            PropertyInfo? endDateProperty = validationContext.ObjectType.GetProperty(EndDatePropertyName);

            DateTime? endDate = (DateTime?)endDateProperty?.GetValue(validationContext.ObjectInstance, null);

            if ((DateTime)value! < endDate) 
            {
                return ValidationResult.Success!;   
            }

            return new ValidationResult(!string.IsNullOrEmpty(ErrorMessage) ? ErrorMessage : $"Date must be before {endDateProperty?.Name}"); // if fail
        }

    }
}
