using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.Core.Models;
using BeaversTests.TestsManager.Infrastructure.DataAccess.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestsManager.Infrastructure.DataAccess;

public class TestsManagerContext(DbContextOptions<TestsManagerContext> options) : DbContext(options), ITestsManagerContext
{
    public DbSet<TestProject> TestProjects { get; init; } = null!;
    public DbSet<BeaversTestPackage> TestPackages { get; init; } = null!;
    public DbSet<TestDriver> TestDrivers { get; init; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TestPackageConfiguration());
        modelBuilder.ApplyConfiguration(new TestProjectConfiguration());
        modelBuilder.ApplyConfiguration(new TestDriverConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}