using MockSchoolManagement.Application.Dtos;

namespace MockSchoolManagement.Application.Students.Dtos
{
    public class GetStudentInput:PagedSortedAndFilterInput
    {
        public GetStudentInput()
        {
            Sorting = "Id";
        }
    }
}
