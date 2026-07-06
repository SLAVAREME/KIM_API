using KIM.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace KIM.DAL.Contexts;

public class KimDbContext(DbContextOptions<KimDbContext> options) : DbContext(options)
{
    public DbSet<Question> Questions => Set<Question>();

    public DbSet<QuestionPackage> QuestionPackages => Set<QuestionPackage>();

    public DbSet<User> Users => Set<User>();

    public DbSet<QuestionUserRating> QuestionUserRatings => Set<QuestionUserRating>();

    public DbSet<PackageUserRating> PackageUserRatings => Set<PackageUserRating>();

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

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Surname).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Birthday).IsRequired();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<QuestionUserRating>(entity =>
        {
            entity.ToTable("QuestionUserRatings");
            entity.HasKey(r => new { r.UserId, r.QuestionId });
            entity.Property(r => r.Value).IsRequired();
            entity
                .HasOne(r => r.User)
                .WithMany(u => u.QuestionRatings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity
                .HasOne(r => r.Question)
                .WithMany(q => q.UserRatings)
                .HasForeignKey(r => r.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PackageUserRating>(entity =>
        {
            entity.ToTable("PackageUserRatings");
            entity.HasKey(r => new { r.UserId, r.PackageId });
            entity.Property(r => r.Value).IsRequired();
            entity
                .HasOne(r => r.User)
                .WithMany(u => u.PackageRatings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity
                .HasOne(r => r.Package)
                .WithMany(p => p.UserRatings)
                .HasForeignKey(r => r.PackageId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}