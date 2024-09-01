using AutoMapper;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.Core.Models;

namespace BeaversTests.TestsManager.App.Mappings;

public class TestDriverMappingProfile : Profile
{
    public TestDriverMappingProfile()
    {
        CreateMap<TestDriver, TestDriverDto>()
            .ReverseMap();
    }
}