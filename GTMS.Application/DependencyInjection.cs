using System.Reflection;
using FluentValidation;
using GTMS.Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace GTMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(new[] { Assembly.GetExecutingAssembly() });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        return services;
    }
}
