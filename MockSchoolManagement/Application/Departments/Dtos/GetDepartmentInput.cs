using MockSchoolManagement.Application.Dtos;

namespace MockSchoolManagement.Application.Departments.Dtos
{
    public class GetDepartmentInput: PagedSortedAndFilterInput
    {
        public GetDepartmentInput()
        {
            Sorting = "Name";
            MaxResultCount = 10;
        }
    }
}
