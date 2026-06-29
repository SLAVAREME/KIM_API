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
}