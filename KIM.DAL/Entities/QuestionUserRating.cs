namespace KIM.DAL.Entities;

public class QuestionUserRating
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid QuestionId { get; set; }

    public Question Question { get; set; } = null!;

    public double Value { get; set; }
}
