using AutoMapper;
using BeaversTests.Common.Binary;
using BeaversTests.TestRunnerAgent.Events;
using BeaversTests.TestsManager.App.Commands;
using BeaversTests.TestsManager.App.Dtos.TestDriver;
using BeaversTests.TestsManager.Core.TestDriver;
using BeaversTests.TestsManager.Events.TestDriver;

namespace BeaversTests.TestsManager.App.Mappings;

public class TestDriverMappingProfile : Profile
{
    public TestDriverMappingProfile()
    {
        CreateMap<NewTestDriverDto, TestDriverAddedEvent>();

        CreateMap<NewTestDriverContentDto, TestDriverContent>();

        CreateMap<TestDriverAddedEvent, TestDriver>()
            .ForMember(dst => dst.UserCreatorId, 
                opt => opt.MapFrom(src => src.UserId));

        CreateMap<RemoveDriverCommand.Command, TestDriverRemovedEvent>();

        CreateMap<TestDriver, TestDriverDto>();

        CreateMap<TestDriverValidationResultEvent, TestDriverValidationStatusEvent>();
    }
}