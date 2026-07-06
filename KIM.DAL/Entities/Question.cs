using System.ComponentModel.DataAnnotations;

namespace KIM.DAL.Entities;

public class Question : BasicModel
{
    [Required]
    [MaxLength(4000)]
    public string Text { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Answer { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Comment { get; set; }

    [MaxLength(255)]
    public string? Author { get; set; }

    public double Rating { get; set; }

    public int RatingVotesCount { get; set; }

    public ICollection<QuestionPackage> QuestionPackages { get; set; } = new List<QuestionPackage>();

    public ICollection<QuestionUserRating> UserRatings { get; set; } = new List<QuestionUserRating>();
}