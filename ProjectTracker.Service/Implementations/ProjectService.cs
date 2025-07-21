using AutoMapper;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Repositories;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectTracker.Service.Implementations
{
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IMapper _mapper;

        public ProjectService(IRepository<Project> projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }

        public async Task<ProjectDto> GetProjectByIdAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            return _mapper.Map<ProjectDto>(project);
        }

        public async Task<int> GetProjectCountAsync()
        {
            return await _projectRepository.CountAsync();
        }

        public async Task<IQueryable<ProjectDto>> GetAllProjectsQueryableAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            var projectDtos = _mapper.Map<IEnumerable<ProjectDto>>(projects);
            return await Task.FromResult(projectDtos.AsQueryable());
        }

        public async Task<ProjectDto> CreateProjectAsync(ProjectDto projectDto)
        {
            var project = _mapper.Map<Project>(projectDto);
            await _projectRepository.AddAsync(project);
            return _mapper.Map<ProjectDto>(project);
        }

        public async Task<ProjectDto> UpdateProjectAsync(int id, ProjectDto projectDto)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                return null;

            _mapper.Map(projectDto, project);
            await _projectRepository.UpdateAsync(project);
            return _mapper.Map<ProjectDto>(project);
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                return false;

            await _projectRepository.DeleteAsync(project);
            return true;
        }

        public async Task<bool> ProjectExistsAsync(int id)
        {
            return await _projectRepository.ExistsAsync(p => p.Id == id);
        }
    }
}