using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Repositories;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectTracker.Service.Services.Implementations
{
    public class UserProjectService : IUserProjectService
    {
        private readonly IRepository<UserProject> _userProjectRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IMapper _mapper;

        public UserProjectService(
            IRepository<UserProject> userProjectRepository,
            IRepository<Project> projectRepository,
            IMapper mapper)
        {
            _userProjectRepository = userProjectRepository;
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<bool> ExistsAsync(int userId, int projectId)
        {
            var list = await _userProjectRepository.GetAsync(up => up.UserId == userId && up.ProjectId == projectId);
            return list.Any();
        }

        public async Task CreateUserProjectAsync(int userId, int projectId)
        {
            // Önce kontrol et, zaten var mı?
            bool exists = await ExistsAsync(userId, projectId);
            if (exists) return; // varsa ekleme yapma

            var userProject = new UserProject
            {
                UserId = userId,
                ProjectId = projectId,
                AssignedDate = DateTime.UtcNow,
                CanView = true, // default izinler
                CanEdit = false,
                CanDelete = false
            };

            await _userProjectRepository.AddAsync(userProject);
        }

        public async Task<IEnumerable<ProjectDto>> GetProjectsByUserIdAsync(int userId)
        {
            var userProjects = await _userProjectRepository
                .GetAsync(up => up.UserId == userId, includes: up => up.Project);

            var projects = userProjects.Select(up => up.Project);

            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }
    }
}
