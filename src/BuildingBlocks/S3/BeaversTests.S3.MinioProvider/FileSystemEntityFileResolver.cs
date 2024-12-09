using BeaversTests.Common.Binary;
using Minio.DataModel;

namespace BeaversTests.S3.MinioProvider;

internal class FileSystemEntityFileResolver
{
    public async Task ResolveFromAsync(
        FileSystemEntityContext context, 
        Item item, 
        Stream objectStream, 
        CancellationToken cancellationToken = default)
    {
        var objectData = GetObjectData(objectStream, cancellationToken);
        
        await ResolveDirectoriesAsync(context, item, objectData);
    }

    private async Task ResolveDirectoriesAsync(FileSystemEntityContext context, Item item, Task<byte[]> data)
    {
        var keySegments = item.Key.Split('/');
        
        var fileName = keySegments.Last();
        var pathSegments = keySegments.SkipLast(1);

        List<BeaversTestsDirectory> directories = context.Directories;
        BeaversTestsDirectory? targetDirectory = null;

        foreach (var pathSegment in pathSegments)
        {
            targetDirectory = directories
                .FirstOrDefault(d => d.DirectoryName == pathSegment);

            if (targetDirectory is null)
            {
                var newDirectory = new BeaversTestsDirectory()
                {
                    DirectoryName = pathSegment,
                    Directories = new List<BeaversTestsDirectory>(),
                    TestFiles = new List<BeaversTestsFile>()
                };
                
                directories.Add(newDirectory);
                directories = (List<BeaversTestsDirectory>)newDirectory.Directories;


                targetDirectory = newDirectory;
                
                continue;
            }
            
            directories = (List<BeaversTestsDirectory>)targetDirectory.Directories;
        }

        var dataObject = await data;
        var file = new BeaversTestsFile()
        {
            Name = fileName,
            Length = dataObject.Length,
            Content = dataObject,
            MediaType = item.ContentType
        };
        
        if (targetDirectory is null)
        {
            context.Files.Add(file);
        }
        else
        {
            ((List<BeaversTestsFile>)targetDirectory.TestFiles).Add(file);
        }
    }
    
    private async Task<byte[]> GetObjectData(
        Stream objectStream, 
        CancellationToken cancellationToken = default)
    {
        await using var ms = new MemoryStream();
        
        await objectStream.CopyToAsync(ms, cancellationToken);
        await objectStream.FlushAsync(cancellationToken);

        await objectStream.DisposeAsync();

        return ms.ToArray();
    }
}