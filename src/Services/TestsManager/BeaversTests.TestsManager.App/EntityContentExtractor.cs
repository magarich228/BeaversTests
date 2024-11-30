using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.App.Dtos.TestPackage;
using BeaversTests.TestsManager.App.Exceptions;

namespace BeaversTests.TestsManager.App;

public class EntityContentExtractor(IServiceProvider serviceProvider)
{
    // TODO: add TryExtract
    public EntityContentDto ExtractContent<TInput>(TInput input)
        where TInput : TestPackageBase
    {
        var extractor = serviceProvider.GetService(typeof(IFileSystemEntityContentExtractor<TInput>)) as IFileSystemEntityContentExtractor<TInput>
            ?? throw new TestsManagerException("File system entity content extractor not found.");
        
        var content = extractor.ExtractContent(input);
        return content;
    }
}