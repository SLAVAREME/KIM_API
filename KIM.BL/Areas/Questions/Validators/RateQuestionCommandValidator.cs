using FluentValidation;
using KIM.BL.Areas.Questions.Commands;

namespace KIM.BL.Areas.Questions.Validators;

public class RateQuestionCommandValidator : AbstractValidator<RateQuestionCommand>
{
    public RateQuestionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Value).InclusiveBetween(0, 5);
    }
}