using AutoMapper;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.Core.TestDriver;

namespace BeaversTests.TestsManager.App.Mappings;

public class TestDriverMappingProfile : Profile
{
    public TestDriverMappingProfile()
    {
        CreateMap<TestDriver, TestDriverDto>()
            .ReverseMap();
    }
}