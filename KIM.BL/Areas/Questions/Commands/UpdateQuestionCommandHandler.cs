using AutoMapper;
using KIM.BL.Areas.Questions.Models;
using KIM.BL.Shared.Exceptions;
using KIM.BL.Shared.Helpers;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Questions.Commands;

public class UpdateQuestionCommandHandler(KimDbContext dbContext, IMapper mapper)
    : IRequestHandler<UpdateQuestionCommand, ApiResponse<QuestionDto>>
{
    public async Task<ApiResponse<QuestionDto>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var current = await dbContext.Questions
            .Include(question => question.QuestionPackages)
            .FirstOrDefaultAsync(question => question.Id == request.Id, cancellationToken);

        if (current is null)
        {
            throw new NotFoundException("Question not found");
        }

        var normalizedText = QuestionMergeHelper.Normalize(request.Text);
        var normalizedAnswer = QuestionMergeHelper.Normalize(request.Answer);

        var duplicate = await dbContext.Questions
            .Include(question => question.QuestionPackages)
            .FirstOrDefaultAsync(
                question => question.Id != request.Id
                            && question.Text.ToLower() == normalizedText
                            && question.Answer.ToLower() == normalizedAnswer,
                cancellationToken);

        if (duplicate is not null)
        {
            duplicate.Author = QuestionMergeHelper.MergeAuthors(duplicate.Author, request.Author);
            duplicate.Comment = QuestionMergeHelper.MergeComment(duplicate.Comment, request.Comment);

            foreach (var package in current.QuestionPackages)
            {
                if (duplicate.QuestionPackages.All(existingPackage => existingPackage.Id != package.Id))
                {
                    duplicate.QuestionPackages.Add(package);
                }
            }

            dbContext.Questions.Remove(current);
            await dbContext.SaveChangesAsync(cancellationToken);

            return ApiResponse<QuestionDto>.SuccessResult(mapper.Map<QuestionDto>(duplicate));
        }

        current.Text = request.Text.Trim();
        current.Answer = request.Answer.Trim();
        current.Author = string.IsNullOrWhiteSpace(request.Author) ? null : request.Author.Trim();
        current.Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);

        return ApiResponse<QuestionDto>.SuccessResult(mapper.Map<QuestionDto>(current));
    }
}