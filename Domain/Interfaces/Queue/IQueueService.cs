namespace Domain.Interfaces.Queue;

public interface IQueueService
{
    public Task InsertQueueMessage(string message);
}