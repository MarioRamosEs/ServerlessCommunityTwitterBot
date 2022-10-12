using Azure.Storage.Queues;
using Domain.Interfaces.Queue;

namespace Infrastructure.Services;

public class QueueService: IQueueService
{
    private readonly QueueClient _queueClient;
    
    public QueueService()
    {
        var connectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("StorageConnectionString configuration not found");
        var queueName = Environment.GetEnvironmentVariable("QueueName");
        if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentException("QueueName configuration not found");
        _queueClient = new QueueClient(connectionString, queueName);
        _queueClient.CreateIfNotExists();
    }
    
    public async Task<string> InsertQueueMessage(string message)
    {
        var response = await _queueClient.SendMessageAsync(messageText: message, timeToLive: new TimeSpan(0,0,-1));
        return response.Value.MessageId;
    }
    
    public async Task<string> GetQueueMessage()
    {
        throw new NotImplementedException();
    }
}