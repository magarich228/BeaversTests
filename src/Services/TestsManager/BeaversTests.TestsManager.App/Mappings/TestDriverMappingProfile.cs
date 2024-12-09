using AutoMapper;
using BeaversTests.Common.Binary;
using BeaversTests.TestsManager.App.Dtos.TestDriver;
using BeaversTests.TestsManager.Events.TestDriver;

namespace BeaversTests.TestsManager.App.Mappings;

public class TestDriverMappingProfile : Profile
{
    public TestDriverMappingProfile()
    {
        CreateMap<NewTestDriverDto, TestDriverAddedEvent>();

        CreateMap<NewTestDriverContentDto, TestDriverContent>();
    }
}