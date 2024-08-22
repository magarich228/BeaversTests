using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace BeaversTests.Client;

// TODO: move to abstractions
public interface ITestsManagerClient
{
    Task<TestPackageIdResponse?> AddTestPackageAsync(
        NewTestPackageDto newTestPackageDto,
        CancellationToken cancellationToken = default);

    Task<TestProjectsResponse?> GetTestProjectsAsync(CancellationToken cancellationToken = default);
}

public class TestsManagerClient(Configuration configuration)
    : BeaversServiceClientBase(configuration),
        ITestsManagerClient
{
    public async Task<TestPackageIdResponse?> AddTestPackageAsync(
        NewTestPackageDto newTestPackageDto,
        CancellationToken cancellationToken = default)
    {
        // ArgumentNullException.ThrowIfNull(newTestPackageDto);
        //
        // // TODO: fix bad request (Name, TestAssemblies, ItemPaths are required)
        // // TODO: get url from OpenApi?
        // var content = new MultipartFormDataContent();
        //
        // content.Add(new StringContent(newTestPackageDto.Name), nameof(newTestPackageDto.Name));
        // content.Add(new StringContent(newTestPackageDto.TestProjectId.ToString()),
        //     nameof(newTestPackageDto.TestProjectId));
        //
        // if (!string.IsNullOrWhiteSpace(newTestPackageDto.Description))
        //     content.Add(new StringContent(newTestPackageDto.Description),
        //         nameof(newTestPackageDto.Description));
        //
        // if (!string.IsNullOrWhiteSpace(newTestPackageDto.TestPackageType))
        //     content.Add(new StringContent(newTestPackageDto.TestPackageType),
        //         nameof(newTestPackageDto.TestPackageType));
        //
        // foreach (var itemPath in newTestPackageDto.ItemPaths)
        // {
        //     content.Add(new StringContent(itemPath), nameof(newTestPackageDto.ItemPaths));
        // }
        //
        // foreach (var file in newTestPackageDto.TestAssemblies)
        // {
        //     // var ms = new MemoryStream();
        //     // await file.CopyToAsync(ms, cancellationToken);
        //     var streamContent = new StreamContent(file.OpenReadStream());
        //     streamContent.Headers.ContentLength = file.Length;
        //     content.Add(streamContent, nameof(newTestPackageDto.TestAssemblies), file.FileName);
        // }
        //
        // var response = await HttpClient.PostAsync("/api/Tests/AddTestPackage", content, cancellationToken);
        //
        // response.EnsureSuccessStatusCode();
        //
        // return await response.Content.ReadFromJsonAsync<TestPackageIdResponse>(cancellationToken);
        
        throw new NotImplementedException();
    }

    public async Task<TestProjectsResponse?> GetTestProjectsAsync(
        CancellationToken cancellationToken = default)
    {
        // TODO: get url from OpenApi?
        var response = await HttpClient.GetAsync("api/Projects/GetAll", cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TestProjectsResponse>(cancellationToken);
    }
}

// TODO: move to contrancts?
public class TestProjectsResponse
{
    public IEnumerable<TestProjectItemResponse> TestProjects { get; init; } = new List<TestProjectItemResponse>();
}

public class TestProjectItemResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; } = null!;
    public string? Description { get; init; }
}

public class NewTestPackageDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required NewTestPackageContentDto Content { get; init; }
    public required string TestPackageType { get; init; }
    public required Guid TestProjectId { get; init; }
}

public class NewTestPackageContentDto
{
    public required IEnumerable<NewTestPackageFileInfo> TestFiles { get; init; }
    public required IEnumerable<NewTestPackageDirectoryDto> Directories { get; init; }
}

public class NewTestPackageFileInfo
{
    public required string Name { get; init; }
    public required long Length { get; init; }
    public required byte[] Content { get; init; }
    public string? MediaType { get; init; }
}

public class NewTestPackageDirectoryDto
{
    public required string DirectoryName { get; init; }
    public required IEnumerable<NewTestPackageFileInfo> TestFiles { get; init; }
    public required IEnumerable<NewTestPackageDirectoryDto> Directories { get; init; }
}

public class TestPackageIdResponse
{
    public Guid TestPackageId { get; set; }
}