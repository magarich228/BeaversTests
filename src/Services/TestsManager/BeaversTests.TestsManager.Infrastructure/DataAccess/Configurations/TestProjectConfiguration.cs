using BeaversTests.TestsManager.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.Configurations;

public class TestProjectConfiguration : IEntityTypeConfiguration<TestProject>
{
    public void Configure(EntityTypeBuilder<TestProject> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasAlternateKey(t => t.Name);

        builder.Property(t => t.Id)
            .IsRequired();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);
    }
}