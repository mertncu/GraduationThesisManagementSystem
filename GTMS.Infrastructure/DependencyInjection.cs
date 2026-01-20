using GTMS.Application.Common.Interfaces;
using GTMS.Infrastructure.Identity;
using GTMS.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GTMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        services.Configure<Identity.JwtSettings>(configuration.GetSection(Identity.JwtSettings.SectionName));
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));

        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IPasswordHasher, PasswordHasher>();
        services.AddTransient<IFileStorageService, FileStorageService>();
        services.AddTransient<IEmailService, SmtpEmailService>();
        
        return services;
    }
}
