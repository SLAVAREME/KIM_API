using AutoMapper;
using KIM.BL.Areas.Questions.Models;
using KIM.BL.Shared.Helpers;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using KIM.DAL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Questions.Commands;

public class CreateQuestionCommandHandler(KimDbContext dbContext, IMapper mapper)
    : IRequestHandler<CreateQuestionCommand, ApiResponse<QuestionDto>>
{
    public async Task<ApiResponse<QuestionDto>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var normalizedText = QuestionMergeHelper.Normalize(request.Text);
        var normalizedAnswer = QuestionMergeHelper.Normalize(request.Answer);

        var existing = await dbContext.Questions.FirstOrDefaultAsync(
            question => question.Text.ToLower() == normalizedText && question.Answer.ToLower() == normalizedAnswer,
            cancellationToken);

        if (existing is not null)
        {
            existing.Author = QuestionMergeHelper.MergeAuthors(existing.Author, request.Author);
            existing.Comment = QuestionMergeHelper.MergeComment(existing.Comment, request.Comment);
            await dbContext.SaveChangesAsync(cancellationToken);

            return ApiResponse<QuestionDto>.SuccessResult(mapper.Map<QuestionDto>(existing));
        }

        var entity = new Question
        {
            Text = request.Text.Trim(),
            Answer = request.Answer.Trim(),
            Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),
            Author = string.IsNullOrWhiteSpace(request.Author) ? null : request.Author.Trim()
        };

        await dbContext.Questions.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ApiResponse<QuestionDto>.SuccessResult(mapper.Map<QuestionDto>(entity));
    }
}