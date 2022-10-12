using System.IO;
using System.Threading.Tasks;
using Domain.Interfaces.Queue;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ServerlessCommunityTwitterBot.Functions;

public class QueueController
{
    private readonly IQueueService _queueService;

    public QueueController(IQueueService queueService)
    {
        _queueService = queueService;
    }

    [FunctionName("InsertIntoQueue")]
    public async Task<IActionResult> InsertIntoQueue([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        string message = req.Query["name"];
        string requestBody;
        using (var streamReader = new StreamReader(req.Body))
        {
            requestBody = await streamReader.ReadToEndAsync();
        }

        dynamic data = JsonConvert.DeserializeObject(requestBody);
        message ??= data?.message;
        if (string.IsNullOrWhiteSpace(message)) return new BadRequestObjectResult("Please pass a name on the query string or in the request body");

        var messageId = await _queueService.InsertQueueMessage(message);

        return new OkObjectResult($"Message inserted into queue, id: {messageId}");
    }
}