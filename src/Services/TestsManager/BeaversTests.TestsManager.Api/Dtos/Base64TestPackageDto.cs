using BeaversTests.TestsManager.App.Abstractions;

namespace BeaversTests.TestsManager.Api.Dtos;

public class Base64TestPackageDto : TestPackageBase
{
    public required IEnumerable<Base64TestPackageFileDto> Base64Files { get; init; }
    public required IEnumerable<Base64TestPackageDirectoryDto> Base64Directories { get; init; }
}

public class Base64TestPackageDirectoryDto
{
    public required string DirectoryName { get; init; }
    public required IEnumerable<Base64TestPackageFileDto> Base64Files { get; init; }
    public required IEnumerable<Base64TestPackageDirectoryDto> Base64Directories { get; init; }
}

public class Base64TestPackageFileDto
{
    public required string Name { get; init; }
    public required string Base64Content { get; init; }
    public required string MediaType { get; init; }
}