using BeaversTests.TestsManager.Core;
using BeaversTests.TestsManager.Core.TestDriver;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.Configurations;

public class TestDriverConfiguration : IEntityTypeConfiguration<TestDriver>
{
    public void Configure(EntityTypeBuilder<TestDriver> builder)
    {
        // TODO: migration for agId, complex key
        builder.HasKey(d => d.Key);
        builder.Property(d => d.Key)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(d => d.AgId)
            .IsRequired();

        builder.Property(d => d.UserCreatorId)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.ValidationResult)
            .IsRequired()
            .HasConversion(
                t => t.ToString(),
                t => Enum.Parse<ValidationResult>(t));
        
        builder.Property(t => t.ValidationMessage)
            .HasDefaultValue(string.Empty)
            .HasMaxLength(1000);
        
        builder.Property(d => d.Description)
            .HasMaxLength(250);
    }
}