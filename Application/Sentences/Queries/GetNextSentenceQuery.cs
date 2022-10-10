using Application.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Sentences.Queries;

public record GetNextSentenceQuery : IRequest<Sentence>;

public class GetNextSentenceQueryHandler : IRequestHandler<GetNextSentenceQuery, Sentence>
{
    private readonly IAppDbContext _context;

    public GetNextSentenceQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Sentence> Handle(GetNextSentenceQuery request, CancellationToken cancellationToken)
    {
        return await (from s in _context.Sentences orderby s.LastUse select s).AsNoTracking().FirstAsync(cancellationToken: cancellationToken);
    }
}
