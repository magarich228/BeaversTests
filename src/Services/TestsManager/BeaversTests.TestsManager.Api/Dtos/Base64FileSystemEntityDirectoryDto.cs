namespace BeaversTests.TestsManager.Api.Dtos;

public class Base64FileSystemEntityDirectoryDto
{
    public required string DirectoryName { get; init; }
    public required IEnumerable<Base64FileSystemEntityFileDto> Base64Files { get; init; }
    public required IEnumerable<Base64FileSystemEntityDirectoryDto> Base64Directories { get; init; }
}