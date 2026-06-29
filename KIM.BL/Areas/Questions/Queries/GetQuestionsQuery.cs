using AutoMapper;
using FluentValidation;
using KIM.BL.Areas.Questions.Models;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Questions.Queries;

public class GetQuestionsQuery : IRequest<ApiResponse<PagedResult<QuestionDto>>>
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public string? Search { get; set; }

    public string SortBy { get; set; } = "text";

    public bool Desc { get; set; }
}

public class GetQuestionsQueryValidator : AbstractValidator<GetQuestionsQuery>
{
    public GetQuestionsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.SortBy).Must(x =>
            x.Equals("text", StringComparison.OrdinalIgnoreCase)
            || x.Equals("answer", StringComparison.OrdinalIgnoreCase)
            || x.Equals("rating", StringComparison.OrdinalIgnoreCase));
    }
}

public class GetQuestionsQueryHandler(KimDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetQuestionsQuery, ApiResponse<PagedResult<QuestionDto>>>
{
    public async Task<ApiResponse<PagedResult<QuestionDto>>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<KIM.DAL.Entities.Question> query = dbContext.Questions.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(question => question.Text.Contains(term) || question.Answer.Contains(term));
        }

        var sortBy = request.SortBy.ToLowerInvariant();
        query = (sortBy, request.Desc) switch
        {
            ("rating", true) => query.OrderByDescending(question => question.Rating),
            ("rating", false) => query.OrderBy(question => question.Rating),
            ("answer", true) => query.OrderByDescending(question => question.Answer),
            ("answer", false) => query.OrderBy(question => question.Answer),
            (_, true) => query.OrderByDescending(question => question.Text),
            _ => query.OrderBy(question => question.Text)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var entities = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = mapper.Map<IReadOnlyCollection<QuestionDto>>(entities);
        var result = new PagedResult<QuestionDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        return ApiResponse<PagedResult<QuestionDto>>.SuccessResult(result);
    }
}