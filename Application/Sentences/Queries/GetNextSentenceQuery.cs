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
        var response = await (from s in _context.Sentences where s.Enabled orderby s.LastUse select s).AsNoTracking().FirstAsync(cancellationToken: cancellationToken);
        if (response == null) throw new ApplicationException("No sentences found");
        return response;
    }
}
