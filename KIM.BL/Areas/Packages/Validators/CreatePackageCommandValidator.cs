using FluentValidation;
using KIM.BL.Areas.Packages.Commands;

namespace KIM.BL.Areas.Packages.Validators;

public class CreatePackageCommandValidator : AbstractValidator<CreatePackageCommand>
{
    public CreatePackageCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Author).MaximumLength(255).When(x => x.Author is not null);
        RuleForEach(x => x.NewQuestions).ChildRules(q =>
        {
            q.RuleFor(x => x.Text).NotEmpty().MaximumLength(2000);
            q.RuleFor(x => x.Answer).NotEmpty().MaximumLength(2000);
        });
    }
}