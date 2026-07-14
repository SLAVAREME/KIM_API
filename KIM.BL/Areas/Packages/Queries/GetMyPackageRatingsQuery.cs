using KIM.BL.Shared.Models;
using KIM.BL.Shared.Responses;
using MediatR;

namespace KIM.BL.Areas.Packages.Queries;

public class GetMyPackageRatingsQuery : IRequest<ApiResponse<IReadOnlyCollection<UserRatingDto>>>
{
    public Guid UserId { get; set; }
}
