using SkylinePlanManagementSystem.Application.Departments.Dtos;
using SkylinePlanManagementSystem.Application.Dtos;
using SkylinePlanManagementSystem.Models;

namespace SkylinePlanManagementSystem.Application.Departments
{
    public interface IDepartmentsService
    {
        /// <summary>
        /// 获取学院的分页信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<Department>> GetPagedDepartmentsList(GetDepartmentInput input);
    }
}
