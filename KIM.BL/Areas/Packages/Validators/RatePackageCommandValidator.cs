using FluentValidation;
using KIM.BL.Areas.Packages.Commands;

namespace KIM.BL.Areas.Packages.Validators;

public class RatePackageCommandValidator : AbstractValidator<RatePackageCommand>
{
    public RatePackageCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Value).InclusiveBetween(0, 5);
    }
}