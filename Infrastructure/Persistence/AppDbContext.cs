using System.Reflection;
using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Sentence> Sentences => Set<Sentence>();

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseCosmos(
    //         "https://community-twitter-bot.documents.azure.com:443/",
    //        "pDZWbf9noUhTeAq2IWNVJ0nfkRajjyolYlIyoFDQ0qk3yJF3P6FnsEqfN9jfgta6PVg34iEjXEtweZya4tI9Xw==",
    //        "CommunityTwitterBot");
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        //modelBuilder.Entity<Sentence>()
        //    .ToContainer("Sencences")
        //    .HasPartitionKey(e => e.Id);
    }
}