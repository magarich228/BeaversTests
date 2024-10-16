using BeaversTests.Common.CQRS;
using BeaversTests.Common.CQRS.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeaversTests.Postgres.EventStore;

public class StreamStateConfiguration : IEntityTypeConfiguration<StreamState>
{
    public void Configure(EntityTypeBuilder<StreamState> builder)
    {
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(p => p.CreatedUtc)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(p => p.AggregateId)
            .IsRequired();

        builder.Property(p => p.Type)
            .IsRequired();

        builder.Property(p => p.Data)
            .IsRequired();

        builder.HasIndex(k => new { k.AggregateId, k.Version });

        builder.HasKey(k => k.Id);
    }
}