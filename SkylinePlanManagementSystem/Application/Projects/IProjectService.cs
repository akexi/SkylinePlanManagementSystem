using SkylinePlanManagementSystem.Application.Dtos;
using SkylinePlanManagementSystem.Application.Projects.Dtos;
using SkylinePlanManagementSystem.Models;
using SkylinePlanManagementSystem.Models;

namespace SkylinePlanManagementSystem.Application.Projects
{
    public interface IProjectService
    {
        Task<PagedResultDto<Project>> GetPaginatedResult(GetProjectInput input);
    }
}
