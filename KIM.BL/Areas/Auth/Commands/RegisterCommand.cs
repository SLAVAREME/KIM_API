using KIM.BL.Areas.Auth.Models;
using KIM.BL.Shared.Responses;
using MediatR;

namespace KIM.BL.Areas.Auth.Commands;

public class RegisterCommand : IRequest<ApiResponse<AuthDto>>
{
    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public DateTime Birthday { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
