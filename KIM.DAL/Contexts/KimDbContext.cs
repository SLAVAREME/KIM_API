using KIM.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace KIM.DAL.Contexts;

public class KimDbContext(DbContextOptions<KimDbContext> options) : DbContext(options)
{
    public DbSet<Question> Questions => Set<Question>();

    public DbSet<QuestionPackage> QuestionPackages => Set<QuestionPackage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("Questions");
            entity.HasKey(question => question.Id);
            entity.Property(question => question.Text).IsRequired().HasMaxLength(4000);
            entity.Property(question => question.Answer).IsRequired().HasMaxLength(2000);
            entity.Property(question => question.Comment).HasMaxLength(4000);
            entity.Property(question => question.Author).HasMaxLength(255);
            entity.Property(question => question.Rating).HasDefaultValue(0).HasPrecision(3, 2);
            entity.Property(question => question.RatingVotesCount).HasDefaultValue(0);

            entity
                .HasMany(question => question.QuestionPackages)
                .WithMany(package => package.Questions)
                .UsingEntity<Dictionary<string, object>>(
                    "QuestionPackageQuestions",
                    right => right
                        .HasOne<QuestionPackage>()
                        .WithMany()
                        .HasForeignKey("QuestionPackageId")
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left
                        .HasOne<Question>()
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.ToTable("QuestionPackageQuestions");
                        join.HasKey("QuestionId", "QuestionPackageId");
                    });
        });

        modelBuilder.Entity<QuestionPackage>(entity =>
        {
            entity.ToTable("QuestionPackages");
            entity.HasKey(package => package.Id);
            entity.Property(package => package.Name).IsRequired().HasMaxLength(255);
            entity.Property(package => package.Author).HasMaxLength(255);
            entity.Property(package => package.Rating).HasDefaultValue(0).HasPrecision(3, 2);
            entity.Property(package => package.RatingVotesCount).HasDefaultValue(0);
            entity.HasIndex(package => package.Name).IsUnique();
        });
    }
}