using FluentValidation;
using KIM.BL.Areas.Questions.Commands;

namespace KIM.BL.Areas.Questions.Validators;

public class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
{
    public UpdateQuestionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Answer).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Comment).MaximumLength(4000);
        RuleFor(x => x.Author).MaximumLength(255);
    }
}