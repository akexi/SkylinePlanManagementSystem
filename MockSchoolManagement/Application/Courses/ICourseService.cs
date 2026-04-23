using SkylinePlanManagementSystem.Application.Dtos;
using SkylinePlanManagementSystem.Models;
using SkylinePlanManagementSystem.Application.Courses.Dtos;

namespace SkylinePlanManagementSystem.Application.Courses
{
    public interface ICourseService
    {
        Task<PagedResultDto<Course>> GetPaginatedResult(GetCourseInput input);
    }
}
