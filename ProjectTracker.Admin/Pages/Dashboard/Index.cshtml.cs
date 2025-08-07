using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Service.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectTracker.Admin.Pages.Dashboard
{
    [Authorize(Roles = "Admin,Manager")]
    public class IndexModel : PageModel
    {
        private readonly IProjectService _projectService;
        public IEnumerable<ProjectDto> Projects { get; set; } = Enumerable.Empty<ProjectDto>();

        public IndexModel(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task OnGetAsync()
        {
            Projects = await _projectService.GetAllProjectsAsync();
        }
    }
}
