using System;
using System.Threading.Tasks;
using Application.Sentences.Queries;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;
using Domain.Interfaces.Twitter;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ServerlessCommunityTwitterBot.Functions;

public class TwitterController
{
    private readonly ISender _mediator;
    private readonly ITwitterSender _twitterSender;

    public TwitterController(ISender mediator, ITwitterSender twitterSender)
    {
        _mediator = mediator;
        _twitterSender = twitterSender;
    }

    [FunctionName("PublishTweet")]
    public async Task PublishTweet([TimerTrigger("0 */12 * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
        var response = await _mediator.Send(new GetNextSentenceQuery());
        await _twitterSender.SendTweet(response.Text);
        await _mediator.Send(new UpdateSentenceCommand()
        {
            Id = response.Id,
            LastUse = DateTime.Now,
            TimesUsed = response.TimesUsed + 1
        });
    }
}