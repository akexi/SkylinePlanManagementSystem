using MockSchoolManagement.Application.Dtos;

namespace MockSchoolManagement.Application.Courses.Dtos
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
