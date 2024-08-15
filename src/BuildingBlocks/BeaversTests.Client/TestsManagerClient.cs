using System.Net.Http.Json;

namespace BeaversTests.Client;

// TODO: move to abstractions
public interface ITestsManagerClient
{
    Task<TestPackageIdResponse?> AddTestPackageAsync(
        NewTestPackageDto newTestPackageDto,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TestProjectResponse>?> GetTestProjectsAsync(CancellationToken cancellationToken = default);
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
        
        // TODO: get url from OpenApi?
        var uri = new Uri("api/Tests/AddTestPackage");
        var response = await HttpClient.PostAsJsonAsync(uri, newTestPackageDto, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<TestPackageIdResponse>(cancellationToken);
    }

    public async Task<IEnumerable<TestProjectResponse>?> GetTestProjectsAsync(
        CancellationToken cancellationToken = default)
    {
        // TODO: get url from OpenApi?
        var uri = new Uri("api/Projects/GetAll");
        var response = await HttpClient.GetAsync(uri, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<IEnumerable<TestProjectResponse>>(cancellationToken);
    }
}

public class TestProjectResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; } = null!;
    public string? Description { get; init; }
}

public class NewTestPackageDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required IEnumerable<byte[]> TestAssemblies { get; init; }
    public required IEnumerable<string> ItemPaths { get; init; }
    public string? TestPackageType { get; init; }
    public required Guid TestProjectId { get; init; }
}

public class TestPackageIdResponse
{
    public Guid TestPackageId { get; set; }
}