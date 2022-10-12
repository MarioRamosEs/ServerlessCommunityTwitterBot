using Domain.Interfaces.Email;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Services;

public class SendGridService: IEmailSender
{
    private readonly SendGridClient _client;
    
    public SendGridService()
    {
        var apiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
        _client = new SendGridClient(apiKey);
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var from = new EmailAddress("mariodarken@gmail.com", "Mario Ramos");
        var to = new EmailAddress(email);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, null, message);
        var response = await _client.SendEmailAsync(msg);
        if (!response.IsSuccessStatusCode) throw new Exception("Error sending email");
    }
}