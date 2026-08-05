using EduAdvisor.Application.Interfaces;
using EduAdvisor.Application.Interfaces.Auth;
using EduAdvisor.Application.Interfaces.ExternalServices;
using EduAdvisor.Application.Interfaces.File;
using EduAdvisor.Domain.Entities.AuthModule;
using EduAdvisor.Infrastructure.ExternalServices.AiRecommendation;
using EduAdvisor.Infrastructure.Localizer;
using EduAdvisor.Infrastructure.Persistence;
using EduAdvisor.Infrastructure.Repositories;
using EduAdvisor.Infrastructure.Services.AuthModules;
using EduAdvisor.Infrastructure.Services.Email;
using EduAdvisor.Infrastructure.Services.File;
using EduAdvisor.Infrastructure.Services.Hasher;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace EduAdvisor.Infrastructure;

public static class DepedencyInjection
{
    #region Public Registration

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
            .AddIdentityConfig()
            .AddHangfireConfig(configuration)
            .AddExternalServices(configuration);
    }

    #endregion

    #region Database

    private static IServiceCollection AddDatabaseConfig(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    #endregion

    #region Persistence

    private static IServiceCollection AddPersistence(
        this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IBaseUrlService, BaseUrlService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IHasherService, HasherService>();
        services.AddScoped<IOtpService, OtpService>();

        services.AddScoped<
            IGetCurrentUserRepository,
            GetCurrentUserRepository>();

        return services;
    }

    #endregion

    #region Localization

    private static IServiceCollection AddLocalizationConfig(
        this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddScoped<IStringLocalizer, JsonStringLocalizer>();

        return services;
    }

    #endregion

    #region Identity

    private static IServiceCollection AddIdentityConfig(
        this IServiceCollection services)
    {
        services.AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    #endregion

    #region External Services

    private static IServiceCollection AddExternalServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var configuredBaseUrl =
            configuration["AiRecommendationService:BaseUrl"]
            ?? throw new InvalidOperationException(
                "AI recommendation service BaseUrl is not configured.");

        var normalizedBaseUrl = configuredBaseUrl.EndsWith('/')
            ? configuredBaseUrl
            : $"{configuredBaseUrl}/";

        if (!Uri.TryCreate(
                normalizedBaseUrl,
                UriKind.Absolute,
                out var baseUri))
        {
            throw new InvalidOperationException(
                "AI recommendation service BaseUrl is invalid.");
        }

        services.AddHttpClient<
            IAiRecommendationService,
            AiRecommendationService>(client =>
            {
                client.BaseAddress = baseUri;
                client.Timeout = TimeSpan.FromSeconds(90);

                client.DefaultRequestHeaders.Accept.ParseAdd(
                    "application/json");
            });

        return services;
    }

    #endregion

    #region Hangfire

    private static IServiceCollection AddHangfireConfig(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found.");

        services.AddHangfire(configurationBuilder =>
        {
            configurationBuilder
                .SetDataCompatibilityLevel(
                    CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(
                    connectionString,
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout =
                            TimeSpan.FromMinutes(5),

                        SlidingInvisibilityTimeout =
                            TimeSpan.FromMinutes(5),

                        QueuePollInterval =
                            TimeSpan.FromSeconds(15),

                        UseRecommendedIsolationLevel =
                            true,

                        DisableGlobalLocks =
                            true
                    });
        });

        services.AddHangfireServer(options =>
        {
            options.ServerName =
                configuration["Hangfire:ServerName"]
                ?? "EduAdvisor.BackupServer";

            options.Queues =
            [
                "emails",
                "default",
                "backups"
            ];
        });

        return services;
    }

    #endregion
}