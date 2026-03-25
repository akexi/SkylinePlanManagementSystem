using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Models;
using MockSchoolManagement.Application.Courses.Dtos;

namespace MockSchoolManagement.Application.Courses
{
    public interface ICourseService
    {
        Task<PagedResultDto<Course>> GetPaginatedResult(GetCourseInput input);
    }
}
