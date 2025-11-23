using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeaversTests.Auth.Persistence;

internal class UserConfiguration : IEntityTypeConfiguration<BeaversUser>
{
    public void Configure(EntityTypeBuilder<BeaversUser> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Email)
            .HasMaxLength(254);

        builder.Property(u => u.DisplayName)
            .HasMaxLength(50);
    }
}