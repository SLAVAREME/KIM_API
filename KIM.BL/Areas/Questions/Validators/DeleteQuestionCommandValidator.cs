using FluentValidation;
using KIM.BL.Areas.Questions.Commands;

namespace KIM.BL.Areas.Questions.Validators;

public class DeleteQuestionCommandValidator : AbstractValidator<DeleteQuestionCommand>
{
    public DeleteQuestionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}