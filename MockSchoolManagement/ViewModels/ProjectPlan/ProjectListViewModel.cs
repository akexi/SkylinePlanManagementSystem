using SkylinePlanManagementSystem.Application.Dtos;
using SkylinePlanManagementSystem.Models;
using SkylinePlanManagementSystem.Models;

namespace SkylinePlanManagementSystem.ViewModels.ProjectPlan
{
    public class ProjectListViewModel
    {
        public PagedResultDto<Project> Projects { get; set; }
    }
}
