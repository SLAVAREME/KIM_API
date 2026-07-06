using KIM.BL.Areas.Auth.Commands;
using KIM.BL.Areas.Auth.Models;
using KIM.BL.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KIM.Api.Areas.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthDto>>> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthDto>>> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }
}
