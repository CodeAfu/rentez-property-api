using System.ComponentModel.DataAnnotations;

namespace RentEZApi.Attributes;

public class MinimumAgeAttribute : ValidationAttribute
{
    private readonly int _minimumAge;

    public MinimumAgeAttribute(int minimumAge)
    {
        _minimumAge = minimumAge;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;
        if (value is DateTime dateOfBirth)
        {
            var age = DateTime.UtcNow.Year - dateOfBirth.Year;
            if (DateTime.UtcNow.DayOfYear < dateOfBirth.DayOfYear) age--;

            if (age >= _minimumAge)
                return ValidationResult.Success;

            return new ValidationResult(
                $"Must be atleast {_minimumAge} years old. Current age: {age}",
                new[] { validationContext.MemberName ?? "DateOfBirth" }
            );
        }

        return new ValidationResult("Invalid date format.");
    }
}
