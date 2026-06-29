namespace KIM.BL.Areas.Questions.Models;

public record QuestionDto(Guid Id, string Text, string Answer, string? Comment, string? Author, double Rating);

public record QuestionLookupDto(Guid Id, string Text, string Answer);