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
        
        CreateMap<TestProjectAddedEvent, TestProject>()
            .ForMember(src => src.UserCreatorId, 
                opt => opt.MapFrom(dst => dst.UserId));
        
        CreateMap<TestProject, TestProjectDto>().ReverseMap();
    }
}