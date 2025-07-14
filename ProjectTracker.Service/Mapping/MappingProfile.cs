using AutoMapper;
using ProjectTracker.Core.Entities;
using ProjectTracker.Service.DTOs;

namespace ProjectTracker.Service.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Project Mappings
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.EmployeeCount,
                    opt => opt.MapFrom(src => src.ProjectEmployees.Count(pe => pe.IsActive)))
                .ForMember(dest => dest.WorkLogCount,
                    opt => opt.MapFrom(src => src.WorkLogs.Count));

            CreateMap<CreateProjectDto, Project>();
            CreateMap<ProjectDto, Project>();

            // WorkLog Mappings
            CreateMap<WorkLog, WorkLogDto>()
                .ForMember(dest => dest.ProjectName,
                    opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
                .ForMember(dest => dest.EmployeeName,
                    opt => opt.MapFrom(src => src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.LastName}" : string.Empty));

            CreateMap<CreateWorkLogDto, WorkLog>();
            CreateMap<WorkLogDto, WorkLog>();

            CreateMap<WorkLogDetail, WorkLogDetailDto>();
            CreateMap<WorkLogAttachment, WorkLogAttachmentDto>();
        }
    }
}