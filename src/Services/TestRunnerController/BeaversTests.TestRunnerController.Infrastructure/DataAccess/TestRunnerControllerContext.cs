using BeaversTests.TestRunnerController.App.Abstractions;
using BeaversTests.TestRunnerController.Core;
using BeaversTests.TestRunnerController.Infrastructure.DataAccess.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestRunnerController.Infrastructure.DataAccess;

public class TestRunnerControllerContext(DbContextOptions<TestRunnerControllerContext> options) : 
    DbContext(options), 
    ITestRunnerControllerContext
{
    public DbSet<TestAgent> TestAgents { get; init; } = null!;
    public DbSet<ControllerUserKey> ControllerUserKeys { get; init; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TestAgentConfiguration());
        modelBuilder.ApplyConfiguration(new ControllerUserKeyConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}