using KIM.BL.Areas.Questions.Commands;
using KIM.BL.Shared.Exceptions;
using KIM.BL.Tests.Infrastructure;
using KIM.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Tests.Questions;

public class QuestionCommandHandlersTests : TestFixtureBase
{
    [Test]
    public async Task CreateQuestion_MergesWithDuplicate_WhenTextAndAnswerAlreadyExists()
    {
        var (dbContext, connection) = CreateDbContext();
        await using var _ = dbContext;
        await using var __ = connection;

        var existing = new Question
        {
            Text = "What is 2+2?",
            Answer = "4",
            Author = "Alice",
            Comment = "old comment"
        };

        await dbContext.Questions.AddAsync(existing);
        await dbContext.SaveChangesAsync();

        var handler = new CreateQuestionCommandHandler(dbContext, CreateMapper());

        var command = new CreateQuestionCommand
        {
            Text = "  What is 2+2?  ",
            Answer = " 4 ",
            Author = "Bob",
            Comment = "new comment"
        };

        var response = await handler.Handle(command, CancellationToken.None);

        Assert.That(response.Success, Is.True);
        Assert.That(response.Data, Is.Not.Null);
        Assert.That(response.Data!.Id, Is.EqualTo(existing.Id));

        var allQuestions = await dbContext.Questions.AsNoTracking().ToListAsync();
        Assert.That(allQuestions, Has.Count.EqualTo(1));

        var question = allQuestions.Single();
        Assert.That(question.Author, Is.EqualTo("Alice; Bob"));
        Assert.That(question.Comment, Is.EqualTo("new comment"));
    }

    [Test]
    public void DeleteQuestion_ThrowsForbidden_WhenQuestionUsedInPackage()
    {
        var (dbContext, connection) = CreateDbContext();
        using var _ = dbContext;
        using var __ = connection;

        var question = new Question
        {
            Text = "Used question",
            Answer = "Answer"
        };

        var package = new QuestionPackage
        {
            Name = "Package"
        };

        package.Questions.Add(question);
        dbContext.QuestionPackages.Add(package);
        dbContext.SaveChanges();

        var handler = new DeleteQuestionCommandHandler(dbContext);

        var action = async () => await handler.Handle(new DeleteQuestionCommand { Id = question.Id }, CancellationToken.None);

        Assert.ThrowsAsync<ForbiddenOperationException>(async () => await action());
        Assert.That(dbContext.Questions.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task RateQuestion_AddsNewRating_WhenUserHasNotRatedBefore()
    {
        var (dbContext, connection) = CreateDbContext();
        await using var _ = dbContext;
        await using var __ = connection;

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Name = "Test", Surname = "User", Birthday = DateTime.UtcNow.AddYears(-20), Email = $"{userId}@test.com", PasswordHash = "hash" };
        var question = new Question { Text = "Q1", Answer = "A1", Rating = 0, RatingVotesCount = 0 };

        await dbContext.Users.AddAsync(user);
        await dbContext.Questions.AddAsync(question);
        await dbContext.SaveChangesAsync();

        var handler = new RateQuestionCommandHandler(dbContext, CreateMapper());
        var command = new RateQuestionCommand { Id = question.Id, UserId = userId, Value = 4.5 };

        var response = await handler.Handle(command, CancellationToken.None);

        Assert.That(response.Success, Is.True);
        Assert.That(response.Data, Is.Not.Null);
        Assert.That(response.Data!.Rating, Is.EqualTo(4.5));

        var userRating = await dbContext.QuestionUserRatings
            .FirstOrDefaultAsync(r => r.UserId == userId && r.QuestionId == question.Id);
        Assert.That(userRating, Is.Not.Null);
        Assert.That(userRating!.Value, Is.EqualTo(4.5));
    }

    [Test]
    public async Task RateQuestion_UpdatesExistingRating_WhenUserAlreadyRated()
    {
        var (dbContext, connection) = CreateDbContext();
        await using var _ = dbContext;
        await using var __ = connection;

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Name = "Test", Surname = "User", Birthday = DateTime.UtcNow.AddYears(-20), Email = $"{userId}@test.com", PasswordHash = "hash" };
        var question = new Question { Text = "Q1", Answer = "A1", Rating = 3.0, RatingVotesCount = 2 };

        await dbContext.Users.AddAsync(user);
        await dbContext.Questions.AddAsync(question);
        await dbContext.SaveChangesAsync();

        var existingRating = new QuestionUserRating { UserId = userId, QuestionId = question.Id, Value = 2.0 };
        await dbContext.QuestionUserRatings.AddAsync(existingRating);
        await dbContext.SaveChangesAsync();

        var handler = new RateQuestionCommandHandler(dbContext, CreateMapper());
        var command = new RateQuestionCommand { Id = question.Id, UserId = userId, Value = 5.0 };

        var response = await handler.Handle(command, CancellationToken.None);

        Assert.That(response.Success, Is.True);
        Assert.That(response.Data, Is.Not.Null);
        Assert.That(response.Data!.Rating, Is.EqualTo(4.5));

        var updatedRating = await dbContext.QuestionUserRatings
            .FirstOrDefaultAsync(r => r.UserId == userId && r.QuestionId == question.Id);
        Assert.That(updatedRating, Is.Not.Null);
        Assert.That(updatedRating!.Value, Is.EqualTo(5.0));
    }

    [Test]
    public async Task RateQuestion_ThrowsNotFound_WhenQuestionDoesNotExist()
    {
        var (dbContext, connection) = CreateDbContext();
        await using var _ = dbContext;
        await using var __ = connection;

        var handler = new RateQuestionCommandHandler(dbContext, CreateMapper());
        var command = new RateQuestionCommand { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Value = 3.0 };

        Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task RateQuestion_CalculatesCorrectAggregate_WhenMultipleUsersRate()
    {
        var (dbContext, connection) = CreateDbContext();
        await using var _ = dbContext;
        await using var __ = connection;

        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var user3 = Guid.NewGuid();

        var userEntity1 = new User { Id = user1, Name = "User1", Surname = "Test", Birthday = DateTime.UtcNow.AddYears(-20), Email = $"{user1}@test.com", PasswordHash = "hash" };
        var userEntity2 = new User { Id = user2, Name = "User2", Surname = "Test", Birthday = DateTime.UtcNow.AddYears(-20), Email = $"{user2}@test.com", PasswordHash = "hash" };
        var userEntity3 = new User { Id = user3, Name = "User3", Surname = "Test", Birthday = DateTime.UtcNow.AddYears(-20), Email = $"{user3}@test.com", PasswordHash = "hash" };
        var question = new Question { Text = "Q1", Answer = "A1", Rating = 0, RatingVotesCount = 0 };

        await dbContext.Users.AddRangeAsync(userEntity1, userEntity2, userEntity3);
        await dbContext.Questions.AddAsync(question);
        await dbContext.SaveChangesAsync();

        var handler = new RateQuestionCommandHandler(dbContext, CreateMapper());

        var cmd1 = new RateQuestionCommand { Id = question.Id, UserId = user1, Value = 5.0 };
        await handler.Handle(cmd1, CancellationToken.None);

        var cmd2 = new RateQuestionCommand { Id = question.Id, UserId = user2, Value = 3.0 };
        var response2 = await handler.Handle(cmd2, CancellationToken.None);

        Assert.That(response2.Data!.Rating, Is.EqualTo(4.0));

        var cmd3 = new RateQuestionCommand { Id = question.Id, UserId = user3, Value = 2.0 };
        var response3 = await handler.Handle(cmd3, CancellationToken.None);

        Assert.That(response3.Data!.Rating, Is.EqualTo(10.0 / 3));
    }
}