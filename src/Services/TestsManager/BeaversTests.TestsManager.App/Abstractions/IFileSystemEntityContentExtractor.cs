using BeaversTests.TestsManager.App.Dtos;

namespace BeaversTests.TestsManager.App.Abstractions;

public interface IFileSystemEntityContentExtractor<in TContent>
{
    // TODO: add async
    EntityContentDto ExtractContent(TContent input);
}