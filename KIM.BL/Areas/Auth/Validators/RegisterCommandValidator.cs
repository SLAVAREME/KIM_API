using FluentValidation;
using KIM.BL.Areas.Auth.Commands;

namespace KIM.BL.Areas.Auth.Validators;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Surname)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Birthday)
            .Must(bd =>
            {
                var age = (DateTime.UtcNow - bd.ToUniversalTime()).TotalDays / 365.25;
                return age >= 6 && age <= 100;
            })
            .WithMessage("Age must be between 6 and 100 years");

        RuleFor(x => x.Email)
            .NotEmpty()
            .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
            .WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters");
    }
}
