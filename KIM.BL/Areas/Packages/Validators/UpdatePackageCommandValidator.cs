using FluentValidation;
using KIM.BL.Areas.Packages.Commands;
using KIM.BL.Areas.Packages.Models;
using KIM.BL.Shared.Helpers;

namespace KIM.BL.Areas.Packages.Validators;

public class UpdatePackageCommandValidator : AbstractValidator<UpdatePackageCommand>
{
    public UpdatePackageCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Author).MaximumLength(255);
        RuleForEach(x => x.NewQuestions).SetValidator(new NewQuestionInputDtoValidator());
        RuleFor(x => x).Custom((command, context) =>
        {
            var duplicates = command.NewQuestions
                .GroupBy(x => $"{QuestionMergeHelper.Normalize(x.Text)}|{QuestionMergeHelper.Normalize(x.Answer)}")
                .Where(group => group.Count() > 1)
                .ToList();

            if (duplicates.Count > 0)
            {
                context.AddFailure("NewQuestions", "New questions contain duplicate text+answer pairs");
            }
        });
    }
}

public class NewQuestionInputDtoValidator : AbstractValidator<NewQuestionInputDto>
{
    public NewQuestionInputDtoValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Answer).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Comment).MaximumLength(4000);
        RuleFor(x => x.Author).MaximumLength(255);
    }
}