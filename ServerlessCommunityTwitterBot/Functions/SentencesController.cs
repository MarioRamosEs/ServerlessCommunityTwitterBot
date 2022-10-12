using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Sentences.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Domain.Exceptions;
using Domain.Models;

namespace ServerlessCommunityTwitterBot.Functions;

public class SentencesController
{
    private readonly ISender _mediator;

    public SentencesController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Disabled in Azure
    /// </summary>
    [FunctionName("CreateSentence")]
    public async Task<IActionResult> CreateSentence([HttpTrigger(AuthorizationLevel.Function, "post", Route = null),
#if RELEASE
                                                      Disable()
#endif
        ]
        HttpRequest req, ILogger log)
    {
        var data = await JsonSerializer.DeserializeAsync<CreateSentenceCommand>(req.Body);
        if (data is null) return new BadRequestResult();
        var sentence = await _mediator.Send(data);
        return new OkObjectResult(sentence);
    }

    /// <summary>
    /// Disabled in Azure
    /// </summary>
    [FunctionName("CreateSentences")]
    public async Task<IActionResult> CreateSentences([HttpTrigger(AuthorizationLevel.Function, "post", Route = null),
#if RELEASE
                                                      Disable()
#endif
        ]
        HttpRequest req, ILogger log)
    {
        var data = await JsonSerializer.DeserializeAsync<List<CreateSentenceCommand>>(req.Body);
        if (data is null || !data.Any()) return new BadRequestResult();
        var response = new List<Sentence>();
        foreach (var sentenceString in data)
        {
            try
            {
                sentenceString.Enabled = true;
                var sentence = await _mediator.Send(sentenceString);
                response.Add(sentence);
            }
            catch (AlreadyExistsException)
            {
                // ignore
            }
        }

        return new OkObjectResult(response);
    }
}