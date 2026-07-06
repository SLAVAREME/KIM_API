namespace KIM.DAL.Entities;

public class PackageUserRating
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid PackageId { get; set; }

    public QuestionPackage Package { get; set; } = null!;

    public double Value { get; set; }
}
