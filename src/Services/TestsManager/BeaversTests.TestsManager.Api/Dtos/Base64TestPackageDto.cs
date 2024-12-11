using BeaversTests.TestsManager.App.Abstractions;

namespace BeaversTests.TestsManager.Api.Dtos;

public class Base64TestPackageDto : TestPackageBase, IEntityBase64Content
{
    public required IEnumerable<Base64FileSystemEntityFileDto> Base64Files { get; init; }
    public required IEnumerable<Base64FileSystemEntityDirectoryDto> Base64Directories { get; init; }
}