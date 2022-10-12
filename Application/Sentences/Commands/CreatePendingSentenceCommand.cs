using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces.Email;
using Domain.Interfaces.Queue;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Sentences.Commands;

public record CreatePendingSentenceCommand : IRequest<string>
{
    public string Text { get; set; } = string.Empty;
}

public class CreatePendingSentenceCommandValidator : AbstractValidator<CreateSentenceCommand>
{
    public CreatePendingSentenceCommandValidator()
    {
        RuleFor(v => v.Text)
            .MaximumLength(280)
            .NotEmpty();
    }
}

public class CreatePendingSentenceCommandHandler : IRequestHandler<CreatePendingSentenceCommand, string>
{
    private readonly IQueueService _queueService;
    private readonly IAppDbContext _context;
    private readonly IEmailSender _emailSender;

    public CreatePendingSentenceCommandHandler(IQueueService queueService, IAppDbContext context, IEmailSender emailSender)
    {
        _queueService = queueService;
        _context = context;
        _emailSender = emailSender;
    }

    public async Task<string> Handle(CreatePendingSentenceCommand request, CancellationToken cancellationToken)
    {
        //Insert into queue
        request.Text = request.Text.Trim();
        var exists = await _context.Sentences.AnyAsync(s => s.Text.Equals(request.Text), cancellationToken);
        if (exists) throw new AlreadyExistsException("Sentence", request.Text);
        var messageId = await _queueService.InsertQueueMessage(request.Text);
        
        //Send email
        var template = GetEmailTemplate(request.Text, messageId);
        await _emailSender.SendEmailAsync("mariodarken@gmail.com", "Twitter Bot - New Sentence", template);
        
        return messageId;
    }

    private string GetEmailTemplate(string requestText, string messageId)
    {
        var emailTemplate = Environment.GetEnvironmentVariable("EmailNewSentenceTemplate");
        if (string.IsNullOrWhiteSpace(emailTemplate)) throw new ArgumentException("EmailNewSentenceTemplate configuration not found");
        var functionAppUrl = Environment.GetEnvironmentVariable("FunctionAppUrl");
        if (string.IsNullOrWhiteSpace(functionAppUrl)) throw new ArgumentException("FunctionAppUrl configuration not found");
        var functionAppHostKey = Environment.GetEnvironmentVariable("FunctionAppHostKey");
        if (string.IsNullOrWhiteSpace(functionAppHostKey)) throw new ArgumentException("FunctionAppHostKey configuration not found");
        
        var resolveLink = $"https://communitytwitterbot.azurewebsites.net/api/ResolvePengingSentence?code={functionAppHostKey}==&messageId={messageId}"; 
        
        emailTemplate = emailTemplate.Replace("[SENTENCE]", requestText);
        emailTemplate = emailTemplate.Replace("[LINK_ACCEPT_SENTENCE]", resolveLink + "&accept=true");
        emailTemplate = emailTemplate.Replace("[LINK_REJECT_SENTENCE]", resolveLink + "&accept=false");
        return emailTemplate;
    }
}


