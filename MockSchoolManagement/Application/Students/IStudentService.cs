using MockSchoolManagement.Models;
using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Application.Students.Dtos;

namespace MockSchoolManagement.Application.Students
{
    public interface IStudentService
    {
        Task<PagedResultDto<Student>> GetPaginatedResult(GetStudentInput input);
    }
}
