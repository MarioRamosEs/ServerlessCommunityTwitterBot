using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class SentenceConfiguration: IEntityTypeConfiguration<Sentence>
{
    public void Configure(EntityTypeBuilder<Sentence> builder)
    {
        builder.ToContainer("Sentences");
        builder.HasPartitionKey(s => s.Id);
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();
        builder.Property(s => s.Text).IsRequired();
        builder.Property(s => s.LastUse).IsRequired();
        builder.Property(s => s.Created).IsRequired();
        builder.Property(s => s.TimesUsed).IsRequired();
    }
}