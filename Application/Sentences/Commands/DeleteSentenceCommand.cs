using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using FluentValidation;
using MediatR;

namespace CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;

public record DeleteSentenceCommand : IRequest
{
    public Guid Id { get; init; }
}

public class DeleteSentenceCommandValidator : AbstractValidator<DeleteSentenceCommand>
{
    public DeleteSentenceCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
    }
}

public class DeleteSentenceCommandHandler : IRequestHandler<DeleteSentenceCommand>
{
    private readonly IAppDbContext _context;

    public DeleteSentenceCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteSentenceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Sentences
            .FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) throw new NotFoundException(nameof(Sentence), request.Id);

        _context.Sentences.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
