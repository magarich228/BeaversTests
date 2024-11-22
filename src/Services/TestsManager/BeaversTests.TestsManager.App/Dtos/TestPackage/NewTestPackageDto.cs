namespace BeaversTests.TestsManager.App.Dtos.TestPackage;

public class NewTestPackageDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required NewTestPackageContentDto Content { get; init; }
    public required string TestDriver { get; init; }
    public required Guid TestProjectId { get; init; }
}

public class NewTestPackageContentDto : EntityContentDto { }