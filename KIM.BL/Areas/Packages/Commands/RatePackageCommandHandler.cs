using AutoMapper;
using KIM.BL.Areas.Packages.Models;
using KIM.BL.Shared.Exceptions;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using KIM.DAL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Packages.Commands;

public class RatePackageCommandHandler(KimDbContext dbContext, IMapper mapper)
    : IRequestHandler<RatePackageCommand, ApiResponse<QuestionPackageDetailsDto>>
{
    public async Task<ApiResponse<QuestionPackageDetailsDto>> Handle(RatePackageCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.QuestionPackages
            .Include(package => package.Questions)
            .FirstOrDefaultAsync(package => package.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Package not found");

        var existing = await dbContext.PackageUserRatings
            .FirstOrDefaultAsync(r => r.UserId == request.UserId && r.PackageId == request.Id, cancellationToken);

        if (existing != null)
        {
            entity.Rating = (entity.Rating * entity.RatingVotesCount - existing.Value + request.Value)
                            / entity.RatingVotesCount;
            existing.Value = request.Value;
        }
        else
        {
            dbContext.PackageUserRatings.Add(new PackageUserRating
            {
                UserId = request.UserId,
                PackageId = request.Id,
                Value = request.Value,
            });

            entity.Rating = ((entity.Rating * entity.RatingVotesCount) + request.Value) / (entity.RatingVotesCount + 1);
            entity.RatingVotesCount += 1;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return ApiResponse<QuestionPackageDetailsDto>.SuccessResult(mapper.Map<QuestionPackageDetailsDto>(entity));
    }
}