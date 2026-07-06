using KIM.BL.Areas.Questions.Models;
using KIM.BL.Shared.Responses;
using MediatR;

namespace KIM.BL.Areas.Questions.Commands;

public class RateQuestionCommand : IRequest<ApiResponse<QuestionDto>>
{
    public Guid Id { get; set; }

    public double Value { get; set; }

    public Guid UserId { get; set; }
}