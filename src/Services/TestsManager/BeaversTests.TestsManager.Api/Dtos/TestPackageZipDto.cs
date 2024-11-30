using BeaversTests.TestsManager.App.Abstractions;

namespace BeaversTests.TestsManager.Api.Dtos;

public class TestPackageZipDto : TestPackageBase, IEntityZipContent
{
    public required IFormFile ZipContent { get; init; }
}