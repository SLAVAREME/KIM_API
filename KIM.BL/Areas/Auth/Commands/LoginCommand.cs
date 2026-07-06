using KIM.BL.Areas.Auth.Models;
using KIM.BL.Shared.Responses;
using MediatR;

namespace KIM.BL.Areas.Auth.Commands;

public class LoginCommand : IRequest<ApiResponse<AuthDto>>
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
