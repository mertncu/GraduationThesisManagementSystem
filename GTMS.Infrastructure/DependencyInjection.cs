using GTMS.Application.Common.Interfaces;
using GTMS.Infrastructure.Identity;
using GTMS.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GTMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IPasswordHasher, PasswordHasher>();
        services.AddTransient<IFileStorageService, FileStorageService>();
        
        return services;
    }
}
