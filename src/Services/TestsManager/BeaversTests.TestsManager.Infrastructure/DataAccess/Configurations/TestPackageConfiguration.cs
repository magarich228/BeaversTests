using BeaversTests.TestsManager.Core.Models;
using BeaversTests.TestsManager.Core.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.Configurations;

public class TestPackageConfiguration : IEntityTypeConfiguration<BeaversTestPackage>
{
    public void Configure(EntityTypeBuilder<BeaversTestPackage> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasAlternateKey(t => new { t.Name, t.TestProjectId });

        builder.Property(t => t.Id)
            .IsRequired();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.TestPackageType)
            .HasMaxLength(50)
            .HasConversion(
                t => t.ToString(),
                t => Enum.Parse<TestPackageType>(t));

        builder.Property(t => t.TestPackage)
            .IsRequired();

        builder.Property(t => t.TestProjectId)
            .IsRequired();

        builder.HasOne<TestProject>(t => t.TestProject)
            .WithMany(t => t.TestPackages)
            .OnDelete(DeleteBehavior.Cascade);
    }
}