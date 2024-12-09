using System.ComponentModel.DataAnnotations;
using BeaversTests.TestsManager.Core.TestPackage;

namespace BeaversTests.TestsManager.Core.TestProject;

public class TestProject
{
    public required Guid Id { get; init; }
    
    [StringLength(50)]
    public required string UserCreatorId { get; init; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public IEnumerable<BeaversTestPackage>? TestPackages { get; init; }
}