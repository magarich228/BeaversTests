using BeaversTests.Client;

namespace BeaversTests.CLI;

public static class TestsManagerExtensions
{
    public static async Task<TestPackageIdResponse?> AddTestPackageFromDirectory(
        this ITestsManagerClient testsManagerClient,
        NewTestPackageFromDirectoryDto newTestPackageDto,
        CancellationToken cancellationToken = default)
    {
        var directoryFiles = newTestPackageDto.Directory
            .GetFiles();
        
        var testPackageDto = new NewTestPackageDto()
        {
            Name = newTestPackageDto.Name,
            TestProjectId = newTestPackageDto.TestProjectId,
            TestPackageType = newTestPackageDto.TestPackageType,
            Description = newTestPackageDto.Description,
            TestAssemblies = directoryFiles.Select(f => File.ReadAllBytes(f.FullName)),
            ItemPaths = directoryFiles.Select(f => f.FullName)
        };

        return await testsManagerClient.AddTestPackageAsync(testPackageDto, cancellationToken);
    }
}

public class NewTestPackageFromDirectoryDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required DirectoryInfo Directory { get; init; }
    public string? TestPackageType { get; init; }
    public required Guid TestProjectId { get; init; }
}