﻿using BeaversTests.TestsManager.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestsManager.App.Abstractions;

public interface ITestsManagerContext
{
    DbSet<TestProject> TestProjects { get; init; }
    DbSet<BeaversTestPackage> TestPackages { get; init; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}