using KIM.BL.Shared.Models;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Questions.Queries;

public class GetMyQuestionRatingsQueryHandler(KimDbContext dbContext)
    : IRequestHandler<GetMyQuestionRatingsQuery, ApiResponse<IReadOnlyCollection<UserRatingDto>>>
{
    public async Task<ApiResponse<IReadOnlyCollection<UserRatingDto>>> Handle(
        GetMyQuestionRatingsQuery request,
        CancellationToken cancellationToken)
    {
        var ratings = await dbContext.QuestionUserRatings
            .Where(r => r.UserId == request.UserId)
            .Select(r => new UserRatingDto(r.QuestionId, r.Value))
            .ToListAsync(cancellationToken);

        return ApiResponse<IReadOnlyCollection<UserRatingDto>>.SuccessResult(ratings);
    }
}
