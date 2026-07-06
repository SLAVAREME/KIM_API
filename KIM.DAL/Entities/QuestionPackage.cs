using System.ComponentModel.DataAnnotations;

namespace KIM.DAL.Entities;

public class QuestionPackage : BasicModel
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Author { get; set; }

    public double Rating { get; set; }

    public int RatingVotesCount { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();

    public ICollection<PackageUserRating> UserRatings { get; set; } = new List<PackageUserRating>();
}