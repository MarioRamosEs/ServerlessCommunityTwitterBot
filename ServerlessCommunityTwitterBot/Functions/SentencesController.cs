using System;
using System.Collections.Generic;
using System.Linq;
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
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;

namespace ServerlessCommunityTwitterBot.Functions;

public class SentencesController
{
    private readonly IAppDbContext _dbContext;
    private readonly ISender _mediator;

    public SentencesController(IAppDbContext dbContext, ISender mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
    }

    [FunctionName("CreateSentence")]
    public async Task<IActionResult> CreateSentence([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        var data = await JsonSerializer.DeserializeAsync<CreateSentenceCommand>(req.Body);
        if (data is null) return new BadRequestResult();
        var sentence = await _mediator.Send(data);
        return new OkObjectResult(sentence);
    }

    [FunctionName("CreateSentences")]
    public async Task<IActionResult> CreateSentences([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        var data = await JsonSerializer.DeserializeAsync<List<CreateSentenceCommand>>(req.Body);
        if (data is null || !data.Any()) return new BadRequestResult();
        var response = new List<Sentence>();
        foreach (var sentenceString in data)
        {
            try
            {
                var sentence = await _mediator.Send(sentenceString);
                response.Add(sentence);
            }
            catch (AlreadyExistsException e)
            {
                // ignore
            }
        }
        return new OkObjectResult(response);
    }

    [FunctionName("GetSentences")]
    public async Task<IActionResult> GetSentences([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
    {
        var sentences = await _dbContext.Sentences.AsNoTracking().ToListAsync(); //TODO make it with MediatR
        return new OkObjectResult(sentences);
    }
}