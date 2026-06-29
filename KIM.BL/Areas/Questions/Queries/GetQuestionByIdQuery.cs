using AutoMapper;
using FluentValidation;
using KIM.BL.Areas.Questions.Models;
using KIM.BL.Shared.Exceptions;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Questions.Queries;

public class GetQuestionByIdQuery : IRequest<ApiResponse<QuestionDto>>
{
    public Guid Id { get; set; }
}

public class GetQuestionByIdQueryValidator : AbstractValidator<GetQuestionByIdQuery>
{
    public GetQuestionByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class GetQuestionByIdQueryHandler(KimDbContext dbContext, IMapper mapper)
    : IRequestHandler<GetQuestionByIdQuery, ApiResponse<QuestionDto>>
{
    public async Task<ApiResponse<QuestionDto>> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(question => question.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException("Question not found");
        }

        return ApiResponse<QuestionDto>.SuccessResult(mapper.Map<QuestionDto>(entity));
    }
}