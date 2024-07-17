﻿using BeaversTests.TestsManager.Core.Models.Enums;

namespace BeaversTests.TestsManager.Core.Models;

public class BeaversTestPackage
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required TestPackageType TestPackageType { get; init; }
    public required Guid TestProjectId { get; init; }
    public TestProject? TestProject { get; init; }
}