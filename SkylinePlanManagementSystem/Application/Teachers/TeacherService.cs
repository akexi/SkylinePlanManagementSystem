using Microsoft.EntityFrameworkCore;
using SkylinePlanManagementSystem.Application.Dtos;
using SkylinePlanManagementSystem.Application.Teachers.Dtos;
using SkylinePlanManagementSystem.Infrastructure.Repositories;
using SkylinePlanManagementSystem.Models;
using System.Linq.Dynamic.Core;

namespace SkylinePlanManagementSystem.Application.Teachers
{
    public class TeacherService:ITeacherService
    {
        private readonly IRepository<Teacher, int> _teacherRepository;

        public TeacherService(IRepository<Teacher, int> teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public async Task<PagedResultDto<Teacher>> GetPagedTeacherList(GetTeacherInput input)
        {
            var query = _teacherRepository.GetAll();

            if(!string.IsNullOrEmpty(input.FilterText))
            {
                query = query.Where(t => t.Name.Contains(input.FilterText));
            }

            // 统计查询数据的总数，用于分页计算总页数
            var count = query.Count();
            // 根据需求进行排序，然后进行分页逻辑的计算
            query = query.OrderBy(input.Sorting)
                .Skip((input.CurrentPage - 1) * input.MaxResultCount)
                .Take(input.MaxResultCount);

            // 将查询结果转换为List集合，加载到内存中
            var models = await query.Include(a => a.OfficeLocation) // 加载导航属性OfficeLocation
                .Include(a => a.CourseAssignments)                  // 加载导航属性CourseAssignments
                .ThenInclude(a => a.Course)                         // 加载CourseAssignments的导航属性Course
                .ThenInclude(a => a.StudentCourses)                 // 加载Course的导航属性StudentCourses
                .ThenInclude(a => a.Student)                        // 加载StudentCourses的导航属性Student
                .Include(i => i.CourseAssignments)                  // 再次加载CourseAssignments导航属性，以便继续加载Course的相关信息
                .ThenInclude(i => i.Course)                         // 加载CourseAssignments的导航属性Course
                .ThenInclude(i => i.Department)                     // 加载Course的导航属性Department
                .AsNoTracking().ToListAsync();

            var dtos = new PagedResultDto<Teacher>
            {
                TotalCount = count,
                CurrentPage = input.CurrentPage,
                MaxResultCount = input.MaxResultCount,
                Data = models,
                FilterText = input.FilterText,
                Sorting = input.Sorting,
            };

            return dtos;
        }



    }
}
