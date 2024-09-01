using BeaversTests.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace BeaversTests.CLI;

public static class TestsManagerExtensions
{
    public static async Task<TestPackageIdResponse?> AddTestPackageFromDirectoryAsync(
        this ITestsManagerClient testsManagerClient,
        NewTestPackageFromDirectoryDto newTestPackageDto,
        CancellationToken cancellationToken = default)
    {
        var rootFiles = new FormFileCollection();
        var rootDirectories = new List<NewTestPackageDirectoryDto>();
        
        var testPackageDto = new NewTestPackageDto()
        {
            Name = newTestPackageDto.Name,
            TestProjectId = newTestPackageDto.TestProjectId,
            TestPackageType = newTestPackageDto.TestPackageType,
            Description = newTestPackageDto.Description,
            Content = new NewTestPackageContentDto(){
                TestFiles = rootFiles,
                Directories = rootDirectories
            }
        };
        
        var directoryFiles = newTestPackageDto.Directory
            .GetFiles();
        
        foreach (var file in directoryFiles)
        {
            AddFileToRequest(rootFiles, file);
        }
        
        var subDirectories = newTestPackageDto.Directory
            .GetDirectories();

        foreach (var subDirectory in subDirectories)
        {
            AddDirectoryToRequest(rootDirectories, subDirectory);
        }
        
        return await testsManagerClient.AddTestPackageAsync(testPackageDto, cancellationToken);
    }

    private static void AddDirectoryToRequest(List<NewTestPackageDirectoryDto> directories, DirectoryInfo directory)
    {
        var directoryFiles = new FormFileCollection();
        var subDirectories = new List<NewTestPackageDirectoryDto>();
        
        foreach (var file in directory.GetFiles())
        {
            AddFileToRequest(directoryFiles, file);
        }

        foreach (var subDirectory in directory.GetDirectories())
        {
            AddDirectoryToRequest(subDirectories, subDirectory);
        }
        
        var newTestPackageDirectory = new NewTestPackageDirectoryDto()
        {
            DirectoryName = directory.Name,
            TestFiles = directoryFiles,
            Directories = subDirectories
        };
        
        directories.Add(newTestPackageDirectory);
    }
    
    private static void AddFileToRequest(FormFileCollection files, FileInfo file)
    {
        var fs = File.OpenRead(file.FullName);
        var formFile = new FormFile(fs, 0, fs.Length, file.Name, file.Name);
        files.Add(formFile);
    }
}