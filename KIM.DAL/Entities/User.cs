using System.ComponentModel.DataAnnotations;

namespace KIM.DAL.Entities;

public class User : BasicModel
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Surname { get; set; } = string.Empty;

    public DateTime Birthday { get; set; }

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<QuestionUserRating> QuestionRatings { get; set; } = new List<QuestionUserRating>();

    public ICollection<PackageUserRating> PackageRatings { get; set; } = new List<PackageUserRating>();
}
