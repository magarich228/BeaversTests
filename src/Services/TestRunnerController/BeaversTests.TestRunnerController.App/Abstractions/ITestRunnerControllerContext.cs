using BeaversTests.TestRunnerController.Core;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestRunnerController.App.Abstractions;

public interface ITestRunnerControllerContext
{
    DbSet<TestAgent> TestAgents { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}