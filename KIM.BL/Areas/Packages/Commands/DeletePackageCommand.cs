using KIM.BL.Shared.Responses;
using MediatR;

namespace KIM.BL.Areas.Packages.Commands;

public class DeletePackageCommand : IRequest<ApiResponse<object>>
{
    public Guid Id { get; set; }
}
