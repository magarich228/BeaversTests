using BeaversTests.TestsManager.Core.Models.Enums;

namespace BeaversTests.TestsManager.Core.Models;

public class BeaversTestPackage
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required byte[] TestPackage { get; set; }
    public required TestPackageType TestPackageType { get; set; }
}