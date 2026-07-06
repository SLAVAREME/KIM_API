using KIM.BL.Areas.Auth.Models;
using KIM.BL.Services;
using KIM.BL.Shared.Exceptions;
using KIM.BL.Shared.Responses;
using KIM.DAL.Contexts;
using KIM.DAL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KIM.BL.Areas.Auth.Commands;

public class RegisterCommandHandler(KimDbContext dbContext, ITokenService tokenService)
    : IRequestHandler<RegisterCommand, ApiResponse<AuthDto>>
{
    public async Task<ApiResponse<AuthDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (exists)
            throw new ConflictException("A user with this email already exists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Surname = request.Surname,
            Birthday = request.Birthday,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

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
