namespace Domain.Interfaces.Queue;

public interface IQueueService
{
    /// <returns>MessageId</returns>
    public Task<string> InsertQueueMessage(string message);
}