using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Interfaces.Auth;
using EduAdvisor.Application.Interfaces.File;
using EduAdvisor.Domain.Entities.AuthModule;
using EduAdvisor.Infrastructure.Localizer;
using EduAdvisor.Infrastructure.Persistence;
using EduAdvisor.Infrastructure.Repositories;
using EduAdvisor.Infrastructure.Services.AuthModules;
using EduAdvisor.Infrastructure.Services.Email;
using EduAdvisor.Infrastructure.Services.File;
using EduAdvisor.Infrastructure.Services.Hasher;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Infrastructure;

public static class DepedencyInjection
{
    public static IServiceCollection AddInfrastructureDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MailSettings>(
            configuration.GetSection("MailSettings"));

        return services
            .AddDatabaseConfig(configuration)
            .AddPersistence()
            .AddLocalizationConfig()
            .AddIdentityConfig();
    }

    private static IServiceCollection AddLocalizationConfig(
        this IServiceCollection services)
    {
        services.AddLocalization();

        services.AddScoped<IStringLocalizer, JsonStringLocalizer>();

        return services;
    }

    public static IServiceCollection AddPersistence(
        this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IBaseUrlService, BaseUrlService>();
        services.AddScoped<IFileStorageService,LocalFileStorageService>();

        services.AddScoped<IHasherService, HasherService>();
        services.AddScoped<IGetCurrentUserRepository, GetCurrentUserRepository>();

        return services;
    }

    private static IServiceCollection AddDatabaseConfig(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    public static IServiceCollection AddIdentityConfig(
        this IServiceCollection services)
    {
        services.AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}