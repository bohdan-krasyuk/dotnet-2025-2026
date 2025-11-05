using Application.Common.Interfaces;
using Infrastructure.BlobStorage;
using Infrastructure.Emails;
using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureInfrastructureServices
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistenceServices(configuration);
        services.AddEmailSendingServices();
        services.AddFileStorageServices();
    }

    private static void AddEmailSendingServices(this IServiceCollection services)
    {
        services.AddScoped<IEmailSendingService, EmailSendingService>();
    }

    private static void AddFileStorageServices(this IServiceCollection services)
    {
        services.AddScoped<IFileStorage, BlobStorageService>();
    }
}