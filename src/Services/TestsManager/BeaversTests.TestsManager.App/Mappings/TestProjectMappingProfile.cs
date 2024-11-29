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
        CreateMap<UpdateProjectCommand.Command, TestProjectUpdatedEvent>();
        CreateMap<TestProjectUpdatedEvent, TestProjectDto>()
            .ForMember(d => d.UserCreatorId, 
                opt => opt.MapFrom(s => s.UserId));
        CreateMap<RemoveProjectCommand.Command, TestProjectDeletedEvent>();
        
        CreateMap<TestProjectAddedEvent, TestProject>();
        CreateMap<TestProjectUpdatedEvent, TestProject>();
        
        CreateMap<TestProject, TestProjectDto>().ReverseMap();
    }
}