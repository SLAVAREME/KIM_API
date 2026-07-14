using AutoMapper;
using KIM.BL.Areas.Packages.Models;
using KIM.BL.Shared.Helpers;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using KIM.DAL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Packages.Commands;

public class CreatePackageCommandHandler(KimDbContext dbContext, IMapper mapper)
    : IRequestHandler<CreatePackageCommand, ApiResponse<QuestionPackageDetailsDto>>
{
    public async Task<ApiResponse<QuestionPackageDetailsDto>> Handle(CreatePackageCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var normalizedName = request.Name.Trim().ToLowerInvariant();
        var existing = await dbContext.QuestionPackages
            .Include(package => package.Questions)
            .FirstOrDefaultAsync(package => package.Name.ToLower() == normalizedName, cancellationToken);

        var entity = existing ?? new QuestionPackage
        {
            Name = request.Name.Trim()
        };

        entity.Author = string.IsNullOrWhiteSpace(request.Author) ? null : request.Author.Trim();

        var requestedExistingIds = request.ExistingQuestionIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        var existingQuestions = await dbContext.Questions
            .Where(question => requestedExistingIds.Contains(question.Id))
            .ToListAsync(cancellationToken);

        var finalQuestions = new List<Question>(existingQuestions);

        foreach (var newQuestion in request.NewQuestions)
        {
            var normalizedText = QuestionMergeHelper.Normalize(newQuestion.Text);
            var normalizedAnswer = QuestionMergeHelper.Normalize(newQuestion.Answer);

            var existingQuestion = await dbContext.Questions.FirstOrDefaultAsync(
                question => question.Text.ToLower() == normalizedText && question.Answer.ToLower() == normalizedAnswer,
                cancellationToken);

            if (existingQuestion is not null)
            {
                existingQuestion.Author = QuestionMergeHelper.MergeAuthors(existingQuestion.Author, newQuestion.Author);
                existingQuestion.Comment = QuestionMergeHelper.MergeComment(existingQuestion.Comment, newQuestion.Comment);
                finalQuestions.Add(existingQuestion);
                continue;
            }

            var created = new Question
            {
                Text = newQuestion.Text.Trim(),
                Answer = newQuestion.Answer.Trim(),
                Comment = string.IsNullOrWhiteSpace(newQuestion.Comment) ? null : newQuestion.Comment.Trim(),
                Author = string.IsNullOrWhiteSpace(newQuestion.Author) ? null : newQuestion.Author.Trim()
            };

            await dbContext.Questions.AddAsync(created, cancellationToken);
            finalQuestions.Add(created);
        }

        var uniqueQuestions = finalQuestions
            .DistinctBy(question => question.Id == Guid.Empty
                ? $"{QuestionMergeHelper.Normalize(question.Text)}|{QuestionMergeHelper.Normalize(question.Answer)}"
                : question.Id.ToString())
            .ToList();

        entity.Questions.Clear();
        foreach (var question in uniqueQuestions)
        {
            entity.Questions.Add(question);
        }

        if (existing is null)
        {
            await dbContext.QuestionPackages.AddAsync(entity, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return ApiResponse<QuestionPackageDetailsDto>.SuccessResult(mapper.Map<QuestionPackageDetailsDto>(entity));
    }
}