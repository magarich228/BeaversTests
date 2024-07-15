using BeaversTests.TestsManager.Core.Models.Enums;

namespace BeaversTests.TestsManager.Core.Models;

public class BeaversTestPackage
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required byte[] TestPackage { get; init; }
    public required TestPackageType TestPackageType { get; init; }
}