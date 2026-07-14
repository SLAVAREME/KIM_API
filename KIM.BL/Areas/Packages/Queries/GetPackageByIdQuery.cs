using AutoMapper;
using FluentValidation;
using KIM.BL.Areas.Packages.Models;
using KIM.BL.Shared.Exceptions;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Packages.Queries;

public class GetPackageByIdQuery : IRequest<ApiResponse<QuestionPackageDetailsDto>>
{
    public Guid Id { get; set; }
}

public class GetPackageByIdQueryValidator : AbstractValidator<GetPackageByIdQuery>
{
    public GetPackageByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class GetPackageByIdQueryHandler(KimDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetPackageByIdQuery, ApiResponse<QuestionPackageDetailsDto>>
{
    public async Task<ApiResponse<QuestionPackageDetailsDto>> Handle(GetPackageByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.QuestionPackages
            .AsNoTracking()
            .Include(package => package.Questions)
            .FirstOrDefaultAsync(package => package.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException("Package not found");
        }

        return ApiResponse<QuestionPackageDetailsDto>.SuccessResult(mapper.Map<QuestionPackageDetailsDto>(entity));
    }
}