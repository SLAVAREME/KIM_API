using KIM.BL.Shared.Responses;
using MediatR;

namespace KIM.BL.Areas.Questions.Commands;

public class DeleteQuestionCommand : IRequest<ApiResponse<object>>
{
    public Guid Id { get; set; }
}
