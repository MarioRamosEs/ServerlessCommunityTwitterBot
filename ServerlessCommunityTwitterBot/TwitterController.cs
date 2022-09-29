using System;
using System.Threading.Tasks;
using Infrastructure.Persistence;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ServerlessCommunityTwitterBot;

public class TwitterController
{
    private readonly AppDbContext _dbContext;

    public TwitterController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [FunctionName("PublishTweet")]
    public async Task PublishTweet([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
    }
}