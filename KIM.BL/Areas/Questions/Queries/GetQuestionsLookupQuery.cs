using FluentValidation;
using KIM.BL.Areas.Questions.Models;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Questions.Queries;

public class GetQuestionsLookupQuery : IRequest<ApiResponse<IReadOnlyCollection<QuestionLookupDto>>>
{
    public string? Search { get; set; }

    public int Limit { get; set; } = 100;
}

public class GetQuestionsLookupQueryValidator : AbstractValidator<GetQuestionsLookupQuery>
{
    public GetQuestionsLookupQueryValidator()
    {
        RuleFor(x => x.Limit).InclusiveBetween(1, 500);
    }
}

public class GetQuestionsLookupQueryHandler(KimDbContext dbContext)
    : IRequestHandler<GetQuestionsLookupQuery, ApiResponse<IReadOnlyCollection<QuestionLookupDto>>>
{
    public async Task<ApiResponse<IReadOnlyCollection<QuestionLookupDto>>> Handle(GetQuestionsLookupQuery request, CancellationToken cancellationToken)
    {
        IQueryable<KIM.DAL.Entities.Question> query = dbContext.Questions.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(question => question.Text.Contains(term) || question.Answer.Contains(term));
        }

        var items = await query
            .OrderBy(question => question.Text)
            .Take(request.Limit)
            .Select(question => new QuestionLookupDto(question.Id, question.Text, question.Answer))
            .ToListAsync(cancellationToken);

        return ApiResponse<IReadOnlyCollection<QuestionLookupDto>>.SuccessResult(items);
    }
}