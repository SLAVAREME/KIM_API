using FluentValidation;
using KIM.BL.Areas.Packages.Models;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Packages.Queries;

public class GetPackagesQuery : IRequest<ApiResponse<PagedResult<QuestionPackageListItemDto>>>
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public string? Search { get; set; }

    public string SortBy { get; set; } = "name";

    public bool Desc { get; set; }
}

public class GetPackagesQueryValidator : AbstractValidator<GetPackagesQuery>
{
    public GetPackagesQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.SortBy).Must(x =>
            x.Equals("name", StringComparison.OrdinalIgnoreCase)
            || x.Equals("author", StringComparison.OrdinalIgnoreCase)
            || x.Equals("rating", StringComparison.OrdinalIgnoreCase));
    }
}

public class GetPackagesQueryHandler(KimDbContext dbContext)
    : IRequestHandler<GetPackagesQuery, ApiResponse<PagedResult<QuestionPackageListItemDto>>>
{
    public async Task<ApiResponse<PagedResult<QuestionPackageListItemDto>>> Handle(GetPackagesQuery request, CancellationToken cancellationToken)
    {
        IQueryable<KIM.DAL.Entities.QuestionPackage> query = dbContext.QuestionPackages.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(package => package.Name.Contains(term));
        }

        var sortBy = request.SortBy.ToLowerInvariant();
        query = (sortBy, request.Desc) switch
        {
            ("rating", true) => query.OrderByDescending(package => package.Rating),
            ("rating", false) => query.OrderBy(package => package.Rating),
            ("author", true) => query.OrderByDescending(package => package.Author),
            ("author", false) => query.OrderBy(package => package.Author),
            (_, true) => query.OrderByDescending(package => package.Name),
            _ => query.OrderBy(package => package.Name)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(package => new QuestionPackageListItemDto(package.Id, package.Name, package.Author, package.Questions.Count, package.Rating))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<QuestionPackageListItemDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        return ApiResponse<PagedResult<QuestionPackageListItemDto>>.SuccessResult(result);
    }
}