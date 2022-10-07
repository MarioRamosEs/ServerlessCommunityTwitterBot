using System;
using System.Reflection;
using Application.Interfaces;
using Domain.Interfaces.Twitter;
using Infrastructure.Persistence;
using Infrastructure.Services;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(ServerlessCommunityTwitterBot.Startup))]

namespace ServerlessCommunityTwitterBot;

class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        //Cosmos DB
        var accountEndpointCosmosDb = Environment.GetEnvironmentVariable("AccountEndpointCosmosDB");
        if (string.IsNullOrWhiteSpace(accountEndpointCosmosDb)) throw new ArgumentNullException(nameof(accountEndpointCosmosDb));
        var accountKeyCosmosDb = Environment.GetEnvironmentVariable("AccountKeyCosmosDB");
        if (string.IsNullOrWhiteSpace(accountKeyCosmosDb)) throw new ArgumentNullException(nameof(accountKeyCosmosDb));
        var databaseNameCosmosDb = Environment.GetEnvironmentVariable("DatabaseNameCosmosDb");
        if (string.IsNullOrWhiteSpace(databaseNameCosmosDb)) throw new ArgumentNullException(nameof(databaseNameCosmosDb));
        builder.Services.AddDbContext<AppDbContext>(
            options => options.UseCosmos(accountEndpointCosmosDb, accountKeyCosmosDb, databaseNameCosmosDb));
        builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        
        //var globalProvider = builder.Services.BuildServiceProvider();
        //using (var context = globalProvider.GetService<AppDbContext>())
        //    context.Database.EnsureCreated();

        //Twitter
        builder.Services.AddScoped<ITwitterSender, TwitterService>();
        
        //MediatR
        builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddScoped(typeof(IAppDbContext), typeof(AppDbContext)); //TODO see if this is needed

        // Configurations
        /*IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Use a singleton Configuration throughout the application
        builder.Services.AddSingleton<IConfiguration>(configuration);*/
    }
}