namespace BeaversTests.TestsManager.Api.Dtos;

public interface IEntityZipContent : IEntityContent
{
    IFormFile ZipContent { get; init; }
}