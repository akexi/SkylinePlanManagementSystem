using SkylinePlanManagementSystem.Application.Dtos;

namespace SkylinePlanManagementSystem.Application.Students.Dtos
{
    public class GetStudentInput:PagedSortedAndFilterInput
    {
        public GetStudentInput()
        {
            Sorting = "Id";
        }
    }
}
