using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MockServer.Core.Entities;
using MockServer.WebMVC.Models.Project;

namespace MockServer.WebMVC.Profiles;

public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<CreateProjectViewModel, Project>().ReverseMap();
        CreateMap<Project, ProjectIndexItem>().ReverseMap();
    }
}
