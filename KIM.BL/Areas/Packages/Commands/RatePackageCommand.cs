using KIM.BL.Areas.Packages.Models;
using KIM.BL.Shared.Responses;
using MediatR;

namespace KIM.BL.Areas.Packages.Commands;

public class RatePackageCommand : IRequest<ApiResponse<QuestionPackageDetailsDto>>
{
    public Guid Id { get; set; }

    public double Value { get; set; }

    // Set by the controller from JWT claims — not bound from request body
    public Guid UserId { get; set; }
}