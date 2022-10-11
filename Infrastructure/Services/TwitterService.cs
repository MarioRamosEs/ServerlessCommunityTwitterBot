using Domain.Interfaces.Twitter;
using Tweetinvi;

namespace Infrastructure.Services;

public class TwitterService: ITwitterSender
{

    private readonly TwitterClient _client;
    
    public TwitterService()
    {
        var apiKey = Environment.GetEnvironmentVariable("TwitterApiKey");
        if (string.IsNullOrWhiteSpace(apiKey)) throw new Exception("Twitter API key not found");
        var apiSecretKey = Environment.GetEnvironmentVariable("TwitterApiSecretKey");
        if (string.IsNullOrWhiteSpace(apiSecretKey)) throw new Exception("Twitter API secret key not found");
        var accessToken = Environment.GetEnvironmentVariable("TwitterAccessToken");
        if (string.IsNullOrWhiteSpace(accessToken)) throw new Exception("Twitter access token not found");
        var accessTokenSecret = Environment.GetEnvironmentVariable("TwitterAccessTokenSecret");
        if (string.IsNullOrWhiteSpace(accessTokenSecret)) throw new Exception("Twitter access token secret not found");
        _client = new TwitterClient(apiKey, apiSecretKey, accessToken, accessTokenSecret);
    }
    
    public async Task SendTweet(string message)
    {
        var publishedTweet = await _client.Tweets.PublishTweetAsync(message);
    }
}