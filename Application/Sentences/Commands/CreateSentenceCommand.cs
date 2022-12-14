using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Sentences.Commands;

public record CreateSentenceCommand : IRequest<Sentence>
{
    public string Text { get; set; } = string.Empty;
    public bool Enabled { get; set; } = false;
}

public class CreateSentenceCommandValidator : AbstractValidator<CreateSentenceCommand>
{
    public CreateSentenceCommandValidator()
    {
        RuleFor(v => v.Text)
            .MaximumLength(280)
            .NotEmpty();
    }
}

public class CreateSentenceCommandHandler : IRequestHandler<CreateSentenceCommand, Sentence>
{
    private readonly IAppDbContext _context;

    public CreateSentenceCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Sentence> Handle(CreateSentenceCommand request, CancellationToken cancellationToken)
    {
        request.Text = request.Text.Trim();
        
        //var exists = await _context.Sentences.AnyAsync(s => s.Text.Equals(request.Text), cancellationToken); //IDK why this doesn't work
        //if (exists) throw new AlreadyExistsException("Sentence already exists.");

        var exists = await _context.Sentences.FirstOrDefaultAsync(s => string.Equals(s.Text, request.Text), cancellationToken);
        if (exists != null) throw new AlreadyExistsException("Sentence", request.Text);
        
        var entity = new Sentence()
        {
            Id = Guid.NewGuid(),
            Text = request.Text,
            Created = DateTime.Now,
            TimesUsed = 0,
            Enabled = request.Enabled
        };
        _context.Sentences.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return entity;
    }
}


