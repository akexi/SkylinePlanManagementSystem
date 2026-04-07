using MockSchoolManagement.Application.Dtos;

namespace MockSchoolManagement.Application.Teachers.Dtos
{
    public class GetTeacherInput:PagedSortedAndFilterInput
    {
        public int? Id { get; set; }
        public int? CourseId { get; set; }
        public GetTeacherInput()
        {
            Sorting = "Id";
            MaxResultCount = 3;
        }
    }
}
