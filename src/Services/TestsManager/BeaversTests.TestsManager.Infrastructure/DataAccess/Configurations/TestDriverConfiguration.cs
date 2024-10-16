using BeaversTests.TestsManager.Core.TestDriver;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.Configurations;

public class TestDriverConfiguration : IEntityTypeConfiguration<TestDriver>
{
    public void Configure(EntityTypeBuilder<TestDriver> builder)
    {
        builder.HasKey(d => d.Key);
        builder.Property(d => d.Key)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(d => d.IsDefault)
            .IsRequired();

        builder.Property(d => d.Description)
            .HasMaxLength(250);
    }
}