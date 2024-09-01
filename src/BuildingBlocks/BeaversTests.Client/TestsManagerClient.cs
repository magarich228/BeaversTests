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
        ArgumentNullException.ThrowIfNull(newTestPackageDto);
        
        // TODO: fix bad request (Content is required) 
        // TODO: get url from OpenApi?
        var requestContent = new MultipartFormDataContent();
        
        requestContent.Add(new StringContent(newTestPackageDto.Name), nameof(newTestPackageDto.Name));
        requestContent.Add(new StringContent(newTestPackageDto.TestProjectId.ToString()),
            nameof(newTestPackageDto.TestProjectId));
        
        if (!string.IsNullOrWhiteSpace(newTestPackageDto.Description))
            requestContent.Add(new StringContent(newTestPackageDto.Description),
                nameof(newTestPackageDto.Description));
        
        if (!string.IsNullOrWhiteSpace(newTestPackageDto.TestPackageType))
            requestContent.Add(new StringContent(newTestPackageDto.TestPackageType),
                nameof(newTestPackageDto.TestPackageType));
        
        var testPackageContent = new MultipartFormDataContent();
        
        foreach (var file in newTestPackageDto.Content.TestFiles)
        {
            AddFileToContent(testPackageContent, file, string.Empty);
        }

        foreach (var directory in newTestPackageDto.Content.Directories)
        {
            var directoryFormContent = new MultipartFormDataContent();
            
            AddDirectoryToContent(directoryFormContent, directory, string.Empty);

            testPackageContent.Add(directoryFormContent, nameof(NewTestPackageContentDto.Directories));
        }
        
        requestContent.Add(testPackageContent, nameof(newTestPackageDto.Content));
        
        var response = await HttpClient.PostAsync("/api/Tests/AddTestPackage", requestContent, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<TestPackageIdResponse>(cancellationToken);
    }

    public async Task<TestProjectsResponse?> GetTestProjectsAsync(
        CancellationToken cancellationToken = default)
    {
        // TODO: get url from OpenApi?
        var response = await HttpClient.GetAsync("api/Projects/GetAll", cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TestProjectsResponse>(cancellationToken);
    }

    private void AddDirectoryToContent(
        MultipartFormDataContent content, 
        NewTestPackageDirectoryDto directory,
        string dirPath)
    {
        var directoryPath = Path.Combine(dirPath, directory.DirectoryName);
        
        foreach (var file in directory.TestFiles)
        {
            AddFileToContent(content, file, directoryPath);
        }

        foreach (var subDirectory in directory.Directories)
        {
            var subDirectoryContent = new MultipartFormDataContent();
            AddDirectoryToContent(content, subDirectory, directoryPath);
            content.Add(subDirectoryContent, nameof(NewTestPackageContentDto.Directories));
        }
    }
    
    private void AddFileToContent(MultipartFormDataContent content, IFormFile file, string dirPath)
    {
        var streamContent = new StreamContent(file.OpenReadStream());
        streamContent.Headers.ContentLength = file.Length;
        content.Add(streamContent, file.Name, Path.Combine(dirPath, file.FileName));
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
    public required IFormFileCollection TestFiles { get; init; }
    public required IEnumerable<NewTestPackageDirectoryDto> Directories { get; init; }
}

public class NewTestPackageDirectoryDto
{
    public required string DirectoryName { get; init; }
    public required IFormFileCollection TestFiles { get; init; }
    public required IEnumerable<NewTestPackageDirectoryDto> Directories { get; init; }
}

public class TestPackageIdResponse
{
    public Guid TestPackageId { get; set; }
}