using BeaversTests.TestsManager.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestsManager.Infrastructure.DataAccess;

public class TestsManagerContext(DbContextOptions<TestsManagerContext> options) : DbContext(options)
{
    public DbSet<TestProject> TestProjects { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}