using BeaversTests.Client;
using Microsoft.AspNetCore.Http.Internal;

namespace BeaversTests.CLI;

public static class TestsManagerExtensions
{
    public static async Task<TestPackageIdResponse?> AddTestPackageFromDirectoryAsync(
        this ITestsManagerClient testsManagerClient,
        NewTestPackageFromDirectoryDto newTestPackageDto,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get dirs
        var directoryFiles = newTestPackageDto.Directory
            .GetFiles();
        var fileCollection = new FormFileCollection();

        foreach (var file in directoryFiles)
        {
            var fs = File.OpenRead(file.FullName);
            var formFile = new FormFile(fs, 0, fs.Length, file.Name, file.Name);
            fileCollection.Add(formFile);
        }
        
        var testPackageDto = new NewTestPackageDto()
        {
            Name = newTestPackageDto.Name,
            TestProjectId = newTestPackageDto.TestProjectId,
            TestPackageType = newTestPackageDto.TestPackageType,
            Description = newTestPackageDto.Description,
            TestAssemblies = fileCollection,
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