using KIM.BL.Shared.Exceptions;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Packages.Commands;

public class DeletePackageCommandHandler(KimDbContext dbContext)
    : IRequestHandler<DeletePackageCommand, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(DeletePackageCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.QuestionPackages
            .Include(package => package.Questions)
            .FirstOrDefaultAsync(package => package.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException("Package not found");
        }

        dbContext.QuestionPackages.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ApiResponse<object>.SuccessResult(null, "Package deleted");
    }
}