using KIM.BL.Areas.Questions.Queries;
using KIM.BL.Tests.Infrastructure;
using KIM.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Tests.Questions;

public class QuestionQueryHandlersTests : TestFixtureBase
{
    [Test]
    public async Task GetMyQuestionRatingsQuery_ReturnsEmptyList_WhenUserHasNoRatings()
    {
        var (dbContext, connection) = CreateDbContext();
        await using var _ = dbContext;
        await using var __ = connection;

        var userId = Guid.NewGuid();

        var handler = new GetMyQuestionRatingsQueryHandler(dbContext);
        var query = new GetMyQuestionRatingsQuery { UserId = userId };

        var response = await handler.Handle(query, CancellationToken.None);

        Assert.That(response.Success, Is.True);
        Assert.That(response.Data, Is.Empty);
    }

    [Test]
    public async Task GetMyQuestionRatingsQuery_ReturnsSingleRating_WhenUserHasOneRating()
    {
        var (dbContext, connection) = CreateDbContext();
        await using var _ = dbContext;
        await using var __ = connection;

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Name = "Test", Surname = "User", Birthday = DateTime.UtcNow.AddYears(-20), Email = $"{userId}@test.com", PasswordHash = "hash" };
        var question = new Question { Text = "Q1", Answer = "A1" };

        await dbContext.Users.AddAsync(user);
        await dbContext.Questions.AddAsync(question);
        await dbContext.SaveChangesAsync();

        var rating = new QuestionUserRating { UserId = userId, QuestionId = question.Id, Value = 4.5 };
        await dbContext.QuestionUserRatings.AddAsync(rating);
        await dbContext.SaveChangesAsync();

        var handler = new GetMyQuestionRatingsQueryHandler(dbContext);
        var query = new GetMyQuestionRatingsQuery { UserId = userId };

        var response = await handler.Handle(query, CancellationToken.None);

        Assert.That(response.Success, Is.True);
        Assert.That(response.Data, Has.Count.EqualTo(1));

        var dto = response.Data!.First();
        Assert.That(dto.ItemId, Is.EqualTo(question.Id));
        Assert.That(dto.Value, Is.EqualTo(4.5));
    }

    [Test]
    public async Task GetMyQuestionRatingsQuery_ReturnsMultipleRatings_WhenUserHasRatedMultiple()
    {
        var (dbContext, connection) = CreateDbContext();
        await using var _ = dbContext;
        await using var __ = connection;

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Name = "Test", Surname = "User", Birthday = DateTime.UtcNow.AddYears(-20), Email = $"{userId}@test.com", PasswordHash = "hash" };
        var q1 = new Question { Text = "Q1", Answer = "A1" };
        var q2 = new Question { Text = "Q2", Answer = "A2" };
        var q3 = new Question { Text = "Q3", Answer = "A3" };

        await dbContext.Users.AddAsync(user);
        await dbContext.Questions.AddRangeAsync(q1, q2, q3);
        await dbContext.SaveChangesAsync();

        var ratings = new[]
        {
            new QuestionUserRating { UserId = userId, QuestionId = q1.Id, Value = 5.0 },
            new QuestionUserRating { UserId = userId, QuestionId = q2.Id, Value = 3.5 },
            new QuestionUserRating { UserId = userId, QuestionId = q3.Id, Value = 2.0 }
        };

        await dbContext.QuestionUserRatings.AddRangeAsync(ratings);
        await dbContext.SaveChangesAsync();

        var handler = new GetMyQuestionRatingsQueryHandler(dbContext);
        var query = new GetMyQuestionRatingsQuery { UserId = userId };

        var response = await handler.Handle(query, CancellationToken.None);

        Assert.That(response.Success, Is.True);
        Assert.That(response.Data, Has.Count.EqualTo(3));

        var dtos = response.Data!.ToList();
        Assert.That(dtos.Select(d => d.ItemId).ToList(), Does.Contain(q1.Id));
        Assert.That(dtos.Select(d => d.ItemId).ToList(), Does.Contain(q2.Id));
        Assert.That(dtos.Select(d => d.ItemId).ToList(), Does.Contain(q3.Id));

        var dto1 = dtos.FirstOrDefault(d => d.ItemId == q1.Id);
        var dto2 = dtos.FirstOrDefault(d => d.ItemId == q2.Id);
        var dto3 = dtos.FirstOrDefault(d => d.ItemId == q3.Id);

        Assert.That(dto1?.Value, Is.EqualTo(5.0));
        Assert.That(dto2?.Value, Is.EqualTo(3.5));
        Assert.That(dto3?.Value, Is.EqualTo(2.0));
    }

    [Test]
    public async Task GetMyQuestionRatingsQuery_IsolatesUserRatings_WhenMultipleUsersRateSameQuestion()
    {
        var (dbContext, connection) = CreateDbContext();
        await using var _ = dbContext;
        await using var __ = connection;

        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();
        var userEntity1 = new User { Id = user1, Name = "User1", Surname = "Test", Birthday = DateTime.UtcNow.AddYears(-20), Email = $"{user1}@test.com", PasswordHash = "hash" };
        var userEntity2 = new User { Id = user2, Name = "User2", Surname = "Test", Birthday = DateTime.UtcNow.AddYears(-20), Email = $"{user2}@test.com", PasswordHash = "hash" };
        var question = new Question { Text = "Q1", Answer = "A1" };

        await dbContext.Users.AddRangeAsync(userEntity1, userEntity2);
        await dbContext.Questions.AddAsync(question);
        await dbContext.SaveChangesAsync();

        var ratings = new[]
        {
            new QuestionUserRating { UserId = user1, QuestionId = question.Id, Value = 5.0 },
            new QuestionUserRating { UserId = user2, QuestionId = question.Id, Value = 2.0 }
        };

        await dbContext.QuestionUserRatings.AddRangeAsync(ratings);
        await dbContext.SaveChangesAsync();

        var handler = new GetMyQuestionRatingsQueryHandler(dbContext);

        var response1 = await handler.Handle(new GetMyQuestionRatingsQuery { UserId = user1 }, CancellationToken.None);
        Assert.That(response1.Data, Has.Count.EqualTo(1));
        Assert.That(response1.Data!.First().Value, Is.EqualTo(5.0));

        var response2 = await handler.Handle(new GetMyQuestionRatingsQuery { UserId = user2 }, CancellationToken.None);
        Assert.That(response2.Data, Has.Count.EqualTo(1));
        Assert.That(response2.Data!.First().Value, Is.EqualTo(2.0));
    }
}
