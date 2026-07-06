using KIM.BL.Areas.Questions.Models;
using KIM.BL.Shared.Responses;
using MediatR;

namespace KIM.BL.Areas.Questions.Commands;

public class RateQuestionCommand : IRequest<ApiResponse<QuestionDto>>
{
    public Guid Id { get; set; }

    public double Value { get; set; }

    // Set by the controller from JWT claims — not bound from request body
    public Guid UserId { get; set; }
}