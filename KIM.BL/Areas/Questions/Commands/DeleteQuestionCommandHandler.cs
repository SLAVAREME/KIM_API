using KIM.BL.Shared.Exceptions;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Questions.Commands;

public class DeleteQuestionCommandHandler(KimDbContext dbContext)
    : IRequestHandler<DeleteQuestionCommand, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var inUse = await dbContext.QuestionPackages
            .AsNoTracking()
            .AnyAsync(package => package.Questions.Any(question => question.Id == request.Id), cancellationToken);

        if (inUse)
        {
            throw new ForbiddenOperationException("Question is used by at least one package and cannot be deleted");
        }

        var entity = await dbContext.Questions.FirstOrDefaultAsync(question => question.Id == request.Id, cancellationToken);
        if (entity is null)
        {
            throw new NotFoundException("Question not found");
        }

        dbContext.Questions.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ApiResponse<object>.SuccessResult(null, "Question deleted");
    }
}