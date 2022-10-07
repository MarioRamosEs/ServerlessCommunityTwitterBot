using System.Threading.Tasks;
using Application.Sentences.Commands;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ServerlessCommunityTwitterBot.Functions;

public class SentencesController
{
    private readonly AppDbContext _dbContext;
    private readonly ISender _mediator;
    public SentencesController(AppDbContext dbContext, ISender mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
    }
    
    [FunctionName("CreateSentence")]
    public async Task<IActionResult> CreateSentence([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        await _dbContext.Database.EnsureCreatedAsync();
        var data = await JsonSerializer.DeserializeAsync<CreateSentenceCommand>(req.Body);
        if (data is null) return new BadRequestResult();
        var sentence = await _mediator.Send(data);
        return new OkObjectResult(sentence);
    }

    [FunctionName("GetSentences")]
    public async Task<IActionResult> GetSentences([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
    {
        var sentences = await _dbContext.Sentences.AsNoTracking().ToListAsync(); //TODO make it with MediatR
        return new OkObjectResult(sentences);
    }
}