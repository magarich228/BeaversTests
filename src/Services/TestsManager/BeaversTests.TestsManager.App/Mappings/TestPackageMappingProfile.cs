﻿using AutoMapper;
using BeaversTests.TestsManager.App.Commands;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.Core.Models;

namespace BeaversTests.TestsManager.App.Mappings;

public class TestPackageMappingProfile : Profile
{
    public TestPackageMappingProfile()
    {
        CreateMap<BeaversTestPackage, AddTestPackageCommand.Command>().ReverseMap();
        CreateMap<BeaversTestPackage, TestPackageDto>().ReverseMap();
    }
}