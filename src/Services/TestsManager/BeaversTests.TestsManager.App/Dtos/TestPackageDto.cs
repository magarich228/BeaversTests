namespace BeaversTests.TestsManager.App.Dtos;

public class TestPackageDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string TestPackageType { get; init; }
}