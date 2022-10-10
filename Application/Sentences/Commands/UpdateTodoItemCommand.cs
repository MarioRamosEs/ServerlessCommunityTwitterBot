using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using FluentValidation;
using MediatR;

namespace CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;

public record UpdateSentenceCommand : IRequest
{
    public Guid Id { get; init; }
    public string? Text { get; set; }
    public int? TimesUsed { get; set; }
    public DateTime? LastUse { get; set; }
}

public class UpdateSentenceCommandValidator : AbstractValidator<UpdateSentenceCommand>
{
    public UpdateSentenceCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.Text)
            .MaximumLength(280);
    }
}

public class UpdateSentenceCommandHandler : IRequestHandler<UpdateSentenceCommand>
{
    private readonly IAppDbContext _context;

    public UpdateSentenceCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateSentenceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Sentences
            .FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) throw new NotFoundException(nameof(Sentence), request.Id);

        if (!string.IsNullOrWhiteSpace(request.Text)) entity.Text = request.Text;
        if (request.LastUse.HasValue) entity.LastUse = request.LastUse.Value;
        if (request.TimesUsed.HasValue) entity.TimesUsed = request.TimesUsed.Value;

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
