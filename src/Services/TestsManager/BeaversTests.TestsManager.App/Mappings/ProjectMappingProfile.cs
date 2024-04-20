using AutoMapper;
using BeaversTests.TestsManager.App.Commands;
using BeaversTests.TestsManager.Core.Models;

namespace BeaversTests.TestsManager.App.Mappings;

public class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        CreateMap<TestProject, CreateProjectCommand>();
    }
}