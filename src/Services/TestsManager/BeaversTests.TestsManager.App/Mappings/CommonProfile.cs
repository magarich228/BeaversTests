using AutoMapper;
using BeaversTests.Common.Binary;
using BeaversTests.TestsManager.App.Dtos;

namespace BeaversTests.TestsManager.App.Mappings;

public class CommonProfile : Profile
{
    public CommonProfile()
    {
        CreateMap<BeaversTestsFileInfo, BeaversTestsFile>();
        CreateMap<BeaversTestsDirectoryInfo, BeaversTestsDirectory>();
        
        CreateMap<EntityContentDto, FileSystemEntity>();
    }
}