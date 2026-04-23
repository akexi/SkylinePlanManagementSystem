using SkylinePlanManagementSystem.Application.Dtos;

namespace SkylinePlanManagementSystem.Application.Projects.Dtos
{
    public class GetProjectInput: PagedSortedAndFilterInput
    {
        public GetProjectInput()
        {
            Sorting = "ProjectId";
            MaxResultCount = 3;
        }
    }
}
