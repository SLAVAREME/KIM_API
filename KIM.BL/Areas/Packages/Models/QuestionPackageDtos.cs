using KIM.BL.Areas.Questions.Models;

namespace KIM.BL.Areas.Packages.Models;

public record QuestionPackageListItemDto(Guid Id, string Name, string? Author, int QuestionsCount, double Rating);

public record QuestionPackageDetailsDto(Guid Id, string Name, string? Author, double Rating, IReadOnlyCollection<QuestionDto> Questions);

public class NewQuestionInputDto
{
    public string Text { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;

    public string? Comment { get; set; }

    public string? Author { get; set; }
}