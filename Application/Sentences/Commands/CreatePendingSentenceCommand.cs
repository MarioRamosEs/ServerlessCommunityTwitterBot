using Domain.Interfaces.Email;
using Domain.Models;
using FluentValidation;
using MediatR;

namespace Application.Sentences.Commands;

public record CreatePendingSentenceCommand : IRequest<Sentence>
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

public class CreatePendingSentenceCommandHandler : IRequestHandler<CreatePendingSentenceCommand, Sentence>
{
    private readonly IEmailSender _emailSender;
    private readonly ISender _mediator;

    public CreatePendingSentenceCommandHandler(IEmailSender emailSender, ISender mediator)
    {
        _emailSender = emailSender;
        _mediator = mediator;
    }

    public async Task<Sentence> Handle(CreatePendingSentenceCommand request, CancellationToken cancellationToken)
    {
        //Insert into queue
        var command = new CreateSentenceCommand
        {
            Text = request.Text,
            Enabled = false
        };
        var sentence = await _mediator.Send(command, cancellationToken);
        
        //Send email
        var template = GetEmailTemplate(request.Text, sentence.Id.ToString());
        await _emailSender.SendEmailAsync("mariodarken@gmail.com", "Twitter Bot - New Sentence", template);
        
        return sentence;
    }

    private string GetEmailTemplate(string requestText, string messageId)
    {
        var emailTemplate = Environment.GetEnvironmentVariable("EmailNewSentenceTemplate");
        if (string.IsNullOrWhiteSpace(emailTemplate)) throw new ArgumentException("EmailNewSentenceTemplate configuration not found");
        var functionAppUrl = Environment.GetEnvironmentVariable("FunctionAppUrl");
        if (string.IsNullOrWhiteSpace(functionAppUrl)) throw new ArgumentException("FunctionAppUrl configuration not found");
        var functionAppHostKey = Environment.GetEnvironmentVariable("FunctionAppHostKey");
        if (string.IsNullOrWhiteSpace(functionAppHostKey)) throw new ArgumentException("FunctionAppHostKey configuration not found");
        
        var resolveLink = $"{functionAppUrl}/api/ResolvePendingSentence?code={functionAppHostKey}&messageId={messageId}"; 
        
        emailTemplate = emailTemplate.Replace("[SENTENCE]", requestText);
        emailTemplate = emailTemplate.Replace("[LINK_ACCEPT_SENTENCE]", resolveLink + "&action=accept");
        emailTemplate = emailTemplate.Replace("[LINK_REJECT_SENTENCE]", resolveLink + "&action=reject");
        return emailTemplate;
    }
}