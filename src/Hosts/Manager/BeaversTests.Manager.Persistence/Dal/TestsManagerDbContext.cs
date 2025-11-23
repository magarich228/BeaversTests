using BeaversTests.Auth;
using BeaversTests.Auth.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.Manager.Persistence.Dal;

public class TestsManagerDbContext(DbContextOptions<TestsManagerDbContext> options) : 
    DbContext(options), 
    IAuthDbContext
{
    public DbSet<BeaversUser> Users { get; init; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}