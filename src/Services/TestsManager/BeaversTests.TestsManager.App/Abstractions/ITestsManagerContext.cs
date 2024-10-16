using BeaversTests.TestsManager.Core.TestDriver;
using BeaversTests.TestsManager.Core.TestPackage;
using BeaversTests.TestsManager.Core.TestProject;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestsManager.App.Abstractions;

public interface ITestsManagerContext
{
    DbSet<TestProject> TestProjects { get; init; }
    DbSet<BeaversTestPackage> TestPackages { get; init; }
    DbSet<TestDriver> TestDrivers { get; init; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}