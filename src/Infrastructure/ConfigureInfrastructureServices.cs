using Application.Common.Interfaces;
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
        services.AddEmailSendingService();
    }

    public static void AddEmailSendingService(this IServiceCollection services)
    {
        services.AddScoped<IEmailSendingService, EmailSendingService>();
    }
}