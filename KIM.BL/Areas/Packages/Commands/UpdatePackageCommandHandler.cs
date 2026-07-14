using AutoMapper;
using KIM.BL.Areas.Packages.Models;
using KIM.BL.Shared.Exceptions;
using KIM.BL.Shared.Helpers;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using KIM.DAL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Packages.Commands;

public class UpdatePackageCommandHandler(KimDbContext dbContext, IMapper mapper)
    : IRequestHandler<UpdatePackageCommand, ApiResponse<QuestionPackageDetailsDto>>
{
    public async Task<ApiResponse<QuestionPackageDetailsDto>> Handle(UpdatePackageCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var current = await dbContext.QuestionPackages
            .Include(package => package.Questions)
            .FirstOrDefaultAsync(package => package.Id == request.Id, cancellationToken);

        if (current is null)
        {
            throw new NotFoundException("Package not found");
        }

        var normalizedName = request.Name.Trim().ToLowerInvariant();
        var packageByName = await dbContext.QuestionPackages
            .Include(package => package.Questions)
            .FirstOrDefaultAsync(
                package => package.Id != request.Id && package.Name.ToLower() == normalizedName,
                cancellationToken);

        var target = packageByName ?? current;
        target.Name = request.Name.Trim();
        target.Author = string.IsNullOrWhiteSpace(request.Author) ? null : request.Author.Trim();

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

            var existing = await dbContext.Questions.FirstOrDefaultAsync(
                question => question.Text.ToLower() == normalizedText && question.Answer.ToLower() == normalizedAnswer,
                cancellationToken);

            if (existing is not null)
            {
                existing.Author = QuestionMergeHelper.MergeAuthors(existing.Author, newQuestion.Author);
                existing.Comment = QuestionMergeHelper.MergeComment(existing.Comment, newQuestion.Comment);
                finalQuestions.Add(existing);
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

        target.Questions.Clear();
        foreach (var question in uniqueQuestions)
        {
            target.Questions.Add(question);
        }

        if (packageByName is not null)
        {
            dbContext.QuestionPackages.Remove(current);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return ApiResponse<QuestionPackageDetailsDto>.SuccessResult(mapper.Map<QuestionPackageDetailsDto>(target));
    }
}