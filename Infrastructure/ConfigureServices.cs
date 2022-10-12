
using Application.Interfaces;
using Domain.Interfaces.Queue;
using Domain.Interfaces.Twitter;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        var accountEndpointCosmosDb = Environment.GetEnvironmentVariable("AccountEndpointCosmosDB");
        if (string.IsNullOrWhiteSpace(accountEndpointCosmosDb)) throw new ArgumentNullException(nameof(accountEndpointCosmosDb));
        var accountKeyCosmosDb = Environment.GetEnvironmentVariable("AccountKeyCosmosDB");
        if (string.IsNullOrWhiteSpace(accountKeyCosmosDb)) throw new ArgumentNullException(nameof(accountKeyCosmosDb));
        var databaseNameCosmosDb = Environment.GetEnvironmentVariable("DatabaseNameCosmosDb");
        if (string.IsNullOrWhiteSpace(databaseNameCosmosDb)) throw new ArgumentNullException(nameof(databaseNameCosmosDb));
        services.AddDbContext<AppDbContext>(
            options => options.UseCosmos(accountEndpointCosmosDb, accountKeyCosmosDb, databaseNameCosmosDb));
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        
        services.AddScoped<ITwitterSender, TwitterService>();
        
        services.AddScoped<IQueueService, QueueService>();
        
        return services;
    }
}
