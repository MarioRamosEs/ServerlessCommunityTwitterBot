using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(ServerlessCommunityTwitterBot.Startup))]

namespace ServerlessCommunityTwitterBot;

class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddInfrastructureServices();
        builder.Services.AddApplicationServices();
    }
}