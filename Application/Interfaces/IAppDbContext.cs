using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Interfaces;

public interface IAppDbContext
{
    public DbSet<Sentence> Sentences { get; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    public DatabaseFacade Database { get; }
}