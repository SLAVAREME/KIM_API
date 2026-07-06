using KIM.BL.Shared.Models;
using KIM.BL.Shared.Responses;
using MediatR;

namespace KIM.BL.Areas.Questions.Queries;

public class GetMyQuestionRatingsQuery : IRequest<ApiResponse<IReadOnlyCollection<UserRatingDto>>>
{
    public Guid UserId { get; set; }
}
