using FluentValidation;
using KIM.BL.Services;
using KIM.BL.Shared.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace KIM.BL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKimBl(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddScoped<ITokenService, TokenService>();

        services.AddAutoMapper(assembly);
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}