using KIM.BL.Areas.Questions.Models;
using KIM.BL.Shared.Responses;
using MediatR;

namespace KIM.BL.Areas.Questions.Commands;

public class CreateQuestionCommand : IRequest<ApiResponse<QuestionDto>>
{
    public string Text { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;

    public string? Comment { get; set; }

    public string? Author { get; set; }
}
