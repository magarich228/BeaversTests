using System.IO.Compression;
using BeaversTests.TestsManager.Api.Dtos;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.App.Exceptions;

namespace BeaversTests.TestsManager.Api.Services;

public class ZipTestPackageContentExtractor : ITestPackageContentExtractor<TestPackageZipDto>
{
    private const string ArchiveReadExceptionMessage =
        "When retrieving the contents of a package, its contet is not read completely";
    
    public NewTestPackageContentDto ExtractContent(TestPackageZipDto input)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));

        var archiveName = Path.GetFileNameWithoutExtension(input.ZipContent.FileName);
        
        var content = input.ZipContent;
        using var zipStream = content.OpenReadStream();
        
        var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, true);
        
        var context = new ExtractionContext()
        {
            RootPath = $"{archiveName}/",
            Files = {  },
            Directories = {  }
        };
        
        foreach (var entry in archive.Entries)
        {
            ProcessEntry(context, entry);
        }
        
        return new NewTestPackageContentDto()
        {
            TestFiles = context.Files,
            Directories = context.Directories
        };
    }

    private void ProcessEntry(ExtractionContext context, ZipArchiveEntry entry)
    {
        if (entry.FullName == context.RootPath)
            return;
            
        if (IsDirectory(entry))
        {
            var directory = new NewTestPackageDirectoryInfo()
            {
                DirectoryName = GetDirectoryName(entry),
                Directories = new List<NewTestPackageDirectoryInfo>(),
                TestFiles = new List<NewTestPackageFileInfo>()
            };
            
            context.AddDirectory(directory, entry.FullName);
                
            return;
        }

        var entryLength = entry.Length;
        byte[] buffer = new byte[entryLength];
            
        using var readStream = entry.Open();
        var bytesRead = readStream.Read(buffer, 0, buffer.Length);

        if (bytesRead != entry.Length)
            throw new TestsManagerException(ArchiveReadExceptionMessage);
                
        var file = new NewTestPackageFileInfo()
        {
            Name = entry.Name,
            Content = buffer,
            Length = bytesRead
        };
            
        context.AddFile(file, entry.FullName);
    }
    
    private bool IsDirectory(ZipArchiveEntry entry) =>
        entry.FullName.EndsWith("/");

    private string GetDirectoryName(ZipArchiveEntry entry)
    {
        var fullName = entry.FullName;

        var index = fullName.LastIndexOf("/", fullName.Length - 2, StringComparison.Ordinal);
        
        return fullName.Substring(index + 1, fullName.Length - index - 2);
    }
    
    private class ExtractionContext
    {
        public required string RootPath { get; init; }
        public List<NewTestPackageFileInfo> Files { get; } = new();
        public List<NewTestPackageDirectoryInfo> Directories { get; } = new();

        public void AddFile(NewTestPackageFileInfo file, string fullPath)
        {
            var searchPath = fullPath
                .Replace(RootPath, string.Empty)
                .Replace($"{file.Name}", string.Empty);
            
            var pathSegments = searchPath.Split('/').Where(s => !string.IsNullOrEmpty(s));
            var filesLevel = Files;
            List<NewTestPackageDirectoryInfo> directoryLevel = Directories;

            foreach (var pathSegment in pathSegments)
            {
                var targetDirectory = directoryLevel.FirstOrDefault(d => d.DirectoryName == pathSegment)
                    ?? throw new TestsManagerException("Could not find directory in archive");
                
                directoryLevel = (List<NewTestPackageDirectoryInfo>)targetDirectory.Directories;
                filesLevel = (List<NewTestPackageFileInfo>)targetDirectory.TestFiles;
            }
            
            filesLevel.Add(file);
        }
        
        public void AddDirectory(NewTestPackageDirectoryInfo directory, string fullPath)
        {
            var searchPath = fullPath
                .Replace(RootPath, string.Empty)
                .Replace($"{directory.DirectoryName}/", string.Empty);

            var pathSegments = searchPath.Split('/').Where(s => !string.IsNullOrEmpty(s));
            var directoryLevel = Directories;

            foreach (var pathSegment in pathSegments)
            {
                var targetDirectory = directoryLevel.FirstOrDefault(d => d.DirectoryName == pathSegment) ??
                                      throw new TestsManagerException("Could not find directory in archive");
                
                directoryLevel = (List<NewTestPackageDirectoryInfo>)targetDirectory.Directories;
            }
            
            directoryLevel.Add(directory);
        }
    }
}