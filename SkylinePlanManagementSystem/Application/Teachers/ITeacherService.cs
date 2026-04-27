using SkylinePlanManagementSystem.Application.Dtos;
using SkylinePlanManagementSystem.Application.Teachers.Dtos;
using SkylinePlanManagementSystem.Models;

namespace SkylinePlanManagementSystem.Application.Teachers
{
    public interface ITeacherService
    {
        /// <summary>
        /// 获取教师的分页信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<Teacher>> GetPagedTeacherList(GetTeacherInput input);
    }
}
