using BeaversTests.TestsManager.App.Abstractions;

namespace BeaversTests.TestsManager.Api.Dtos;

public class TestDriverZipDto : TestDriverBase, IEntityZipContent
{
    public required IFormFile ZipContent { get; init; }
}