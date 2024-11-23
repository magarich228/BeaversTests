using AutoMapper;
using BeaversTests.TestsManager.App.Commands;
using BeaversTests.TestsManager.App.Dtos.TestProject;
using BeaversTests.TestsManager.Core.TestProject;
using BeaversTests.TestsManager.Events.TestProject;

namespace BeaversTests.TestsManager.App.Mappings;

public class TestProjectMappingProfile : Profile
{
    public TestProjectMappingProfile()
    {
        CreateMap<CreateProjectCommand.Command, TestProjectAddedEvent>();
        CreateMap<TestProjectAddedEvent, TestProject>();
        CreateMap<TestProjectUpdatedEvent, TestProject>();
        // CreateMap<TestProject, UpdateProjectCommand.Command>().ReverseMap();
        CreateMap<TestProject, TestProjectDto>().ReverseMap();
    }
}