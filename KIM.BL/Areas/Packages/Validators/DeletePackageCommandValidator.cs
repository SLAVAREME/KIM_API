using FluentValidation;
using KIM.BL.Areas.Packages.Commands;

namespace KIM.BL.Areas.Packages.Validators;

public class DeletePackageCommandValidator : AbstractValidator<DeletePackageCommand>
{
    public DeletePackageCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}