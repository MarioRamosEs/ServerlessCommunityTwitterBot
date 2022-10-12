using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Application.Sentences.Commands;

namespace ServerlessCommunityTwitterBot.Functions;

public class PendingSentencesController
{
    private readonly ISender _mediator;

    public PendingSentencesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [FunctionName("CreatePendingSentence")]
    public async Task<IActionResult> CreatePendingSentence([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        var data = await JsonSerializer.DeserializeAsync<CreatePendingSentenceCommand>(req.Body);
        if (data is null) return new BadRequestResult();
        var messageId = await _mediator.Send(data);
        return new OkObjectResult($"Message inserted into queue, id: {messageId}");
    }
}