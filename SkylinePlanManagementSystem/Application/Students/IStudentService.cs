using SkylinePlanManagementSystem.Models;
using SkylinePlanManagementSystem.Application.Dtos;
using SkylinePlanManagementSystem.Application.Students.Dtos;

namespace SkylinePlanManagementSystem.Application.Students
{
    public interface IStudentService
    {
        Task<PagedResultDto<Student>> GetPaginatedResult(GetStudentInput input);
    }
}
