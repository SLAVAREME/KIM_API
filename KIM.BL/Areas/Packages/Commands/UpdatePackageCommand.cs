using KIM.BL.Areas.Packages.Models;
using KIM.BL.Shared.Responses;
using MediatR;

namespace KIM.BL.Areas.Packages.Commands;

public class UpdatePackageCommand : IRequest<ApiResponse<QuestionPackageDetailsDto>>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Author { get; set; }

    public IReadOnlyCollection<Guid> ExistingQuestionIds { get; set; } = Array.Empty<Guid>();

    public IReadOnlyCollection<NewQuestionInputDto> NewQuestions { get; set; } = Array.Empty<NewQuestionInputDto>();
}