using KIM.BL.Shared.Models;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Packages.Queries;

public class GetMyPackageRatingsQueryHandler(KimDbContext dbContext)
    : IRequestHandler<GetMyPackageRatingsQuery, ApiResponse<IReadOnlyCollection<UserRatingDto>>>
{
    public async Task<ApiResponse<IReadOnlyCollection<UserRatingDto>>> Handle(
        GetMyPackageRatingsQuery request,
        CancellationToken cancellationToken)
    {
        var ratings = await dbContext.PackageUserRatings
            .Where(r => r.UserId == request.UserId)
            .Select(r => new UserRatingDto(r.PackageId, r.Value))
            .ToListAsync(cancellationToken);

        return ApiResponse<IReadOnlyCollection<UserRatingDto>>.SuccessResult(ratings);
    }
}
