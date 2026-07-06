using AutoMapper;
using KIM.BL.Areas.Questions.Models;
using KIM.BL.Shared.Exceptions;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using KIM.DAL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Questions.Commands;

public class RateQuestionCommandHandler(KimDbContext dbContext, IMapper mapper)
    : IRequestHandler<RateQuestionCommand, ApiResponse<QuestionDto>>
{
    public async Task<ApiResponse<QuestionDto>> Handle(RateQuestionCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Questions
            .FirstOrDefaultAsync(question => question.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Question not found");

        var existing = await dbContext.QuestionUserRatings
            .FirstOrDefaultAsync(r => r.UserId == request.UserId && r.QuestionId == request.Id, cancellationToken);

        if (existing != null)
        {
            // Update: recalculate aggregate without changing vote count
            entity.Rating = (entity.Rating * entity.RatingVotesCount - existing.Value + request.Value)
                            / entity.RatingVotesCount;
            existing.Value = request.Value;
        }
        else
        {
            dbContext.QuestionUserRatings.Add(new QuestionUserRating
            {
                UserId = request.UserId,
                QuestionId = request.Id,
                Value = request.Value,
            });

            entity.Rating = ((entity.Rating * entity.RatingVotesCount) + request.Value) / (entity.RatingVotesCount + 1);
            entity.RatingVotesCount += 1;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return ApiResponse<QuestionDto>.SuccessResult(mapper.Map<QuestionDto>(entity));
    }
}