using MockSchoolManagement.Models;

namespace MockSchoolManagement.Application.Students
{
    public interface IStudentService
    {
        Task<List<Student>> GetPaginatedResult(int currentPage, string searchString, string sortBy, int pageSize = 10);
    }
}
