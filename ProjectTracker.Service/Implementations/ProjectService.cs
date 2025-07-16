using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using ProjectTracker.Data.Repositories;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Services.Interfaces;

namespace ProjectTracker.Service.Services.Implementations
{
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly AppDbContext _context; // Bu satırı ekleyin
        private readonly IMapper _mapper;

        public ProjectService(IRepository<Project> projectRepository, AppDbContext context, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _context = context; // Bu satırı ekleyin
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }

        public async Task<IQueryable<ProjectDto>> GetAllProjectsQueryableAsync()
        {
            var projects = _context.Projects
                .Where(p => p.IsActive)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Budget = p.Budget,
                    ActualCost = p.ActualCost
                });

            return await Task.FromResult(projects); // await ekledik
        }

        public async Task<ProjectDto?> GetProjectByIdAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            return _mapper.Map<ProjectDto>(project);
        }

        public async Task<ProjectDto> CreateProjectAsync(ProjectDto projectDto)
        {
            var project = _mapper.Map<Project>(projectDto);
            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveAsync();
            return _mapper.Map<ProjectDto>(project);
        }

        public async Task<ProjectDto?> UpdateProjectAsync(int id, ProjectDto projectDto)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null) return null;

            _mapper.Map(projectDto, project);
            _projectRepository.Update(project);
            await _projectRepository.SaveAsync();

            return _mapper.Map<ProjectDto>(project);
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null) return false;

            _projectRepository.Remove(project);
            await _projectRepository.SaveAsync();
            return true;
        }

        public async Task<int> GetProjectCountAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            return projects.Count();
        }
    }
}