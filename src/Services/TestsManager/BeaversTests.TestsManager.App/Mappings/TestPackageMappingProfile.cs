using AutoMapper;
using BeaversTests.Common.Binary;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.App.Dtos.TestPackage;
using BeaversTests.TestsManager.Core;
using BeaversTests.TestsManager.Core.TestPackage;

namespace BeaversTests.TestsManager.App.Mappings;

public class TestPackageMappingProfile : Profile
{
    public TestPackageMappingProfile()
    {
        CreateMap<BeaversTestPackage, NewTestPackageDto>()
            .ForMember(dst => dst.TestDriver,
                opt => opt.MapFrom(src => src.TestDriverKey));

        CreateMap<NewTestPackageDto, BeaversTestPackage>()
            .ForMember(dst => dst.TestDriver,
                opt => opt.Ignore())
            .ForMember(dst => dst.TestDriverKey,
                opt => opt.MapFrom(src => src.TestDriver));
        
        CreateMap<BeaversTestPackage, TestPackageDto>().ReverseMap();

        CreateMap<BeaversTestsFile, BeaversTestsFileInfo>().ReverseMap();
        CreateMap<BeaversTestsDirectory, BeaversTestsDirectoryInfo>().ReverseMap();
        CreateMap<NewTestPackageContentDto, TestPackageContent>().ReverseMap();
    }
}