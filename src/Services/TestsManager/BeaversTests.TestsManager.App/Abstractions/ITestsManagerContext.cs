using BeaversTests.TestsManager.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestsManager.App.Abstractions;

public interface ITestsManagerContext
{ 
    DbSet<TestProject> TestProjects { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}