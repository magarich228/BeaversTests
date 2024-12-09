using BeaversTests.Common.Binary;

namespace BeaversTests.S3.MinioProvider;

internal class FileSystemEntityContext
{
    public List<BeaversTestsFile> Files { get; } = new();
    public List<BeaversTestsDirectory> Directories { get; } = new();
    
    public FileSystemEntity ToEntity<TEntity>() 
        where TEntity : FileSystemEntity, new() => new TEntity()
    {
        Files = Files,
        Directories = Directories
    };
}