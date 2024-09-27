using AutoMapper;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.Core.Models;

namespace BeaversTests.TestsManager.App.Mappings;

public class TestPackageMappingProfile : Profile
{
    public TestPackageMappingProfile()
    {
        CreateMap<BeaversTestPackage, NewTestPackageDto>().ReverseMap();
        
        CreateMap<BeaversTestPackage, TestPackageDto>().ReverseMap();

        CreateMap<TestPackageFile, NewTestPackageFileInfo>().ReverseMap();
        CreateMap<TestPackageContentDirectory, NewTestPackageDirectoryInfo>();
        CreateMap<TestPackageContent, NewTestPackageContentDto>().ReverseMap();
    }
}