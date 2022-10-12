using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;
using FluentValidation;
using MediatR;

namespace Application.Sentences.Commands;

public enum ResolvePendingSentenceCommandAction
{
    Accept,
    Reject,
}

public record ResolvePendingSentenceCommand : IRequest
{
    public Guid Id { get; set; }
    public ResolvePendingSentenceCommandAction Action { get; set; }
}

public class ResolvePendingSentenceCommandValidator : AbstractValidator<ResolvePendingSentenceCommand>
{
    public ResolvePendingSentenceCommandValidator()
    {
        RuleFor(v => v.Action).NotEmpty();
        RuleFor(v => v.Id).NotEmpty();
    }
}

public class ResolvePendingSentenceCommandHandler : IRequestHandler<ResolvePendingSentenceCommand>
{

    private readonly ISender _mediator;
    public ResolvePendingSentenceCommandHandler(ISender mediator)
    {
        _mediator = mediator;
    }

    public async Task<Unit> Handle(ResolvePendingSentenceCommand request, CancellationToken cancellationToken)
    {
        switch (request.Action)
        {
            case ResolvePendingSentenceCommandAction.Accept:
                var updateSentenceCommand = new UpdateSentenceCommand
                {
                    Id = request.Id,
                    Enabled = true
                };
                await _mediator.Send(updateSentenceCommand, cancellationToken);
                break;
            case ResolvePendingSentenceCommandAction.Reject:
                var deleteSentenceCommand = new DeleteSentenceCommand
                {
                    Id = request.Id,
                };
                await _mediator.Send(deleteSentenceCommand, cancellationToken);
                break;
            default:
                throw new ApplicationException("Invalid action");
        }
        return Unit.Value;
    }
}