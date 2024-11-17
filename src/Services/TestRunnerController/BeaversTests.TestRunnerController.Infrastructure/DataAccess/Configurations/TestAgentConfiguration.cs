using BeaversTests.TestRunnerController.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeaversTests.TestRunnerController.Infrastructure.DataAccess.Configurations;

public class TestAgentConfiguration : IEntityTypeConfiguration<TestAgent>
{
    public void Configure(EntityTypeBuilder<TestAgent> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion(t => t.ToString(),
                t => Enum.Parse<TestAgentStatus>(t))
            .IsRequired();
    }
}