using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VectorysInventaire.Application.Behaviors;
using VectorysInventaire.Application.Features.Auth.Commands.Login;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Application.Services;

namespace VectorysInventaire.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<IScanCompletionService, ScanCompletionService>();
        return services;
    }
}
