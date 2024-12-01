namespace BeaversTests.Common.Binary;

public class BeaversTestsFile
{
    public required string Name { get; init; }
    public required long Length { get; init; }
    public required byte[] Content { get; init; }
    public string? MediaType { get; init; }
}