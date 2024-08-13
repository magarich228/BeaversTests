using System.Net.Http.Json;

namespace BeaversTests.Client;

// TODO: move to abstractions
public interface ITestsManagerClient
{
    Task<TestPackageIdResponse?> AddTestPackageAsync(
        NewTestPackageDto newTestPackageDto,
        CancellationToken cancellationToken = default);
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