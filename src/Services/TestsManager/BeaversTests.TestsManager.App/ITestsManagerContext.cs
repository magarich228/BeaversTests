using BeaversTests.TestsManager.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestsManager.App;

public interface ITestsManagerContext
{ 
    DbSet<TestProject> TestProjects { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}