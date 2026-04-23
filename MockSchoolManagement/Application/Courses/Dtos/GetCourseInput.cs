using SkylinePlanManagementSystem.Application.Dtos;

namespace SkylinePlanManagementSystem.Application.Courses.Dtos
{
    public class GetCourseInput: PagedSortedAndFilterInput
    {
        public GetCourseInput()
        {
            Sorting = "CourseId";
            MaxResultCount = 3;
        }
    }
}
