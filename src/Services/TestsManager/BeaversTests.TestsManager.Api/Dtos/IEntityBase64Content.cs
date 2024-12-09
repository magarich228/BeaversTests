namespace BeaversTests.TestsManager.Api.Dtos;

public interface IEntityBase64Content
{
    IEnumerable<Base64FileSystemEntityFileDto> Base64Files { get; init; }
    IEnumerable<Base64FileSystemEntityDirectoryDto> Base64Directories { get; init; }
}