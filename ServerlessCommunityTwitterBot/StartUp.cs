using System;
using Infrastructure.Persistence;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(ServerlessCommunityTwitterBot.Startup))]

namespace ServerlessCommunityTwitterBot;

class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var accountEndpointCosmosDb = Environment.GetEnvironmentVariable("AccountEndpointCosmosDB");
        if (string.IsNullOrWhiteSpace(accountEndpointCosmosDb)) throw new ArgumentNullException(nameof(accountEndpointCosmosDb));
        var accountKeyCosmosDb = Environment.GetEnvironmentVariable("AccountKeyCosmosDB");
        if (string.IsNullOrWhiteSpace(accountKeyCosmosDb)) throw new ArgumentNullException(nameof(accountKeyCosmosDb));
        var databaseNameCosmosDb = Environment.GetEnvironmentVariable("DatabaseNameCosmosDb");
        if (string.IsNullOrWhiteSpace(databaseNameCosmosDb)) throw new ArgumentNullException(nameof(databaseNameCosmosDb));
        builder.Services.AddDbContext<AppDbContext>(
            options => options.UseCosmos(accountEndpointCosmosDb, accountKeyCosmosDb, databaseNameCosmosDb));
    }
}