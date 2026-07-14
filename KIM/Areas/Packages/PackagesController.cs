using KIM.BL.Areas.Packages.Commands;
using KIM.BL.Areas.Packages.Models;
using KIM.BL.Areas.Packages.Queries;
using KIM.BL.Shared.Models;
using KIM.BL.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace KIM.Api.Areas.Packages;

[ApiController]
[Route("api/packages")]
public class PackagesController(IMediator mediator) : ControllerBase
{
    [HttpPost("rating")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<QuestionPackageDetailsDto>>> Rate(
        [FromBody] RatePackageCommand command,
        CancellationToken cancellationToken)
    {
        command.UserId = Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpGet("my-ratings")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<UserRatingDto>>>> GetMyRatings(CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        var response = await mediator.Send(new GetMyPackageRatingsQuery { UserId = userId }, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<QuestionPackageListItemDto>>>> Get([FromQuery] GetPackagesQuery query, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<QuestionPackageDetailsDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetPackageByIdQuery { Id = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<QuestionPackageDetailsDto>>> Create([FromBody] CreatePackageCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<QuestionPackageDetailsDto>>> Update(Guid id, [FromBody] UpdatePackageCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeletePackageCommand { Id = id }, cancellationToken);
        return Ok(response);
    }
}