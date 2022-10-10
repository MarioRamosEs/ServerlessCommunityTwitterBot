namespace Domain.Interfaces.Twitter;

public interface ITwitterSender
{
    public Task SendTweet(string message);
}