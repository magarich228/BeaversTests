using AutoMapper;
using BeaversTests.TestRunnerController.App.Dtos;
using BeaversTests.TestRunnerController.Core;

namespace BeaversTests.TestRunnerController.App.Mappings;

public class TestAgentProfile : Profile
{
    public TestAgentProfile()
    {
        CreateMap<TestAgent, TestAgentDto>()
            .ForMember(
                dto => dto.Status, 
                opt => opt.MapFrom(a => a.Status.ToString()))
            .ForMember(dto => dto.Id, opt => opt.MapFrom(a => a.Id));
        
        CreateMap<TestAgentDto, TestAgent>()
            .ForMember(
                a => a.Status, 
                opt => opt.MapFrom(dto => Enum.Parse<TestAgentStatus>(dto.Status)))
            .ForMember(a => a.Id, opt => opt.MapFrom(dto => dto.Id));
    }
}