using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IAppDbContext
{
    DbSet<Sentence> Sentences { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}