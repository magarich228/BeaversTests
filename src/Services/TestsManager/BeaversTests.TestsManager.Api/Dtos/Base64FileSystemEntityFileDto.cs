namespace BeaversTests.TestsManager.Api.Dtos;

public class Base64FileSystemEntityFileDto
{
    public required string Name { get; init; }
    public required string Base64Content { get; init; }
    public required string MediaType { get; init; }
}