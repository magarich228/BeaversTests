using BeaversTests.TestsManager.App.Abstractions;

namespace BeaversTests.TestsManager.Api.Dtos;

public class TestPackageZipDto : TestPackageBase
{
    public required IFormFile ZipContent { get; init; }
}