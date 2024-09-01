namespace BeaversTests.TestsManager.App.Dtos;

public class NewTestPackageFileInfo
{
    public required string Name { get; init; }
    public required long Length { get; init; }
    public required byte[] Content { get; init; }
    public string? MediaType { get; init; }
}