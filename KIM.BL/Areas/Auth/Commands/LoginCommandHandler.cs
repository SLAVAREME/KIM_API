using KIM.BL.Areas.Auth.Models;
using KIM.BL.Services;
using KIM.BL.Shared.Exceptions;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Auth.Commands;

public class LoginCommandHandler(KimDbContext dbContext, ITokenService tokenService)
    : IRequestHandler<LoginCommand, ApiResponse<AuthDto>>
{
    public async Task<ApiResponse<AuthDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken)
            ?? throw new NotFoundException("Invalid email or password");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new NotFoundException("Invalid email or password");

        var token = tokenService.GenerateToken(user);

        return ApiResponse<AuthDto>.SuccessResult(new AuthDto
        {
            Token = token,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
        });
    }
}
