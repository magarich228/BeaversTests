using AutoMapper;
using BeaversTests.TestsManager.App.Commands;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.Core.TestProject;

namespace BeaversTests.TestsManager.App.Mappings;

public class TestProjectMappingProfile : Profile
{
    public TestProjectMappingProfile()
    {
        CreateMap<TestProject, CreateProjectCommand.Command>().ReverseMap();
        CreateMap<TestProject, UpdateProjectCommand.Command>().ReverseMap();
        CreateMap<TestProject, TestProjectDto>().ReverseMap();
    }
}