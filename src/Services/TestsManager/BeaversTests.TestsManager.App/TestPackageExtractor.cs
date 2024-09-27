using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.App.Exceptions;

namespace BeaversTests.TestsManager.App;

public class TestPackageExtractor(IServiceProvider serviceProvider)
{
    public NewTestPackageDto ExtractTestPackage<TInput>(TInput input)
        where TInput : TestPackageBase
    {
        var extractor = serviceProvider.GetService(typeof(ITestPackageContentExtractor<TInput>)) as ITestPackageContentExtractor<TInput>
            ?? throw new TestsManagerException("Test package content extractor not found.");
        
        var content = extractor.ExtractContent(input);
        
        return new NewTestPackageDto()
        {
            Name = input.Name,
            Description = input.Description,
            TestDriver = input.TestPackageType,
            TestProjectId = input.TestProjectId,
            Content = content
        };
    }
}