using AutoMapper;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.Core.Models;
using BeaversTests.TestsManager.Core.Models.Enums;

namespace BeaversTests.TestsManager.App.Mappings;

public class TestPackageMappingProfile : Profile
{
    public TestPackageMappingProfile()
    {
        CreateMap<BeaversTestPackage, NewTestPackageDto>()
            .ForMember(t => t.TestPackageType,
                cfg => cfg.MapFrom(t => t.TestPackageType.ToString()));
        
        CreateMap<NewTestPackageDto, BeaversTestPackage>()
            .ForMember(t => t.TestPackageType,
                cfg => cfg.MapFrom(t => Enum.Parse<TestPackageType>(t.TestPackageType)));
        
        CreateMap<BeaversTestPackage, TestPackageDto>().ReverseMap();

        CreateMap<TestPackageFile, NewTestPackageFileInfo>().ReverseMap();
        CreateMap<TestPackageContentDirectory, NewTestPackageDirectoryDto>();
        CreateMap<TestPackageContent, NewTestPackageContentDto>().ReverseMap();
    }
}