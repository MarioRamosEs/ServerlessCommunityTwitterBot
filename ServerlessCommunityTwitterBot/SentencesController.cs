using System;
using System.Threading.Tasks;
using Infrastructure.Persistence;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ServerlessCommunityTwitterBot;

public class SentencesController
{
    private readonly AppDbContext _dbContext;
    public SentencesController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [FunctionName("PrintSentences")]
    public async Task PrintSentences([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
        var sentences = await _dbContext.Sentences.AsNoTracking().ToListAsync();
        log.LogInformation($"{sentences.Count} sentences found");
        var jsonSentences = JsonConvert.SerializeObject(sentences);
        log.LogInformation(jsonSentences);
    }
}