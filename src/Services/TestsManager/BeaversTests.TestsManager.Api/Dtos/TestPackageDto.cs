using BeaversTests.TestsManager.App.Abstractions;

namespace BeaversTests.TestsManager.Api.Dtos;

public class TestPackageDto : TestPackageBase
{
    public required TestPackageContentDto Content { get; init; }
}

public class TestPackageContentDto
{
    public required IFormFileCollection TestFiles { get; init; }
    public required IEnumerable<TestPackageDirectoryDto> Directories { get; init; }
}