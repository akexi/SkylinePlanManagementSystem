using SkylinePlanManagementSystem.Application.Dtos;

namespace SkylinePlanManagementSystem.Application.Departments.Dtos
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
