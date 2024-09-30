using BeaversTests.TestsManager.Api.Dtos;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;

namespace BeaversTests.TestsManager.Api.Services;

// TODO: Протестить
public class Base64TestPackageContentExtractor : ITestPackageContentExtractor<Base64TestPackageDto>
{
    public NewTestPackageContentDto ExtractContent(Base64TestPackageDto input)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));

        IEnumerable<NewTestPackageFileInfo> files = input.Base64Files.Select(GetFile);
        IEnumerable<NewTestPackageDirectoryInfo> directories = input.Base64Directories.Select(GetDirectory);
        
        var result = new NewTestPackageContentDto()
        {
            TestFiles = files,
            Directories = directories
        };

        return result;
    }

    private NewTestPackageFileInfo GetFile(Base64TestPackageFileDto base64File)
    {
        var content = Convert.FromBase64String(base64File.Base64Content);
        
        return new NewTestPackageFileInfo()
        {
            Name = base64File.Name,
            Length = content.Length,
            Content = content,
            MediaType = base64File.MediaType
        };
    }

    private NewTestPackageDirectoryInfo GetDirectory(Base64TestPackageDirectoryDto base64Directory)
    {
        return new NewTestPackageDirectoryInfo()
        {
            DirectoryName = base64Directory.DirectoryName,
            Directories = base64Directory.Base64Directories.Select(GetDirectory),
            TestFiles = base64Directory.Base64Files.Select(GetFile)
        };
    }
}