using BeaversTests.TestRunnerController.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeaversTests.TestRunnerController.Infrastructure.DataAccess.Configurations;

public class ControllerUserKeyConfiguration : IEntityTypeConfiguration<ControllerUserKey>
{
    public void Configure(EntityTypeBuilder<ControllerUserKey> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Key)
            .IsRequired();
        
        builder.Property(c => c.OwnerId)
            .IsRequired();
    }
}