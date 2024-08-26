using BeaversTests.TestsManager.Core.Models;
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

        builder.Property(t => t.TestDriverKey)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(t => t.TestProjectId)
            .IsRequired();

        // On delete?
        builder.HasOne<TestDriver>(t => t.TestDriver)
            .WithMany(d => d.TestPackages);
        
        builder.HasOne<TestProject>(t => t.TestProject)
            .WithMany(t => t.TestPackages)
            .OnDelete(DeleteBehavior.Cascade);
    }
}