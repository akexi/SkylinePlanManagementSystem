using Microsoft.EntityFrameworkCore;
using SkylinePlanManagementSystem.Application.Departments.Dtos;
using SkylinePlanManagementSystem.Application.Dtos;
using SkylinePlanManagementSystem.Infrastructure.Repositories;
using SkylinePlanManagementSystem.Models;
using System.Globalization;
using System.Linq.Dynamic.Core;

namespace SkylinePlanManagementSystem.Application.Departments
{
    public class DepartmentsService : IDepartmentsService
    {
        private readonly IRepository<Department, int> _departmentRepository;

        public DepartmentsService(IRepository<Department, int> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<PagedResultDto<Department>> GetPagedDepartmentsList(GetDepartmentInput input)
        {
            var query = _departmentRepository.GetAll();

            if (!string.IsNullOrEmpty(input.FilterText))
            {
                query = query.Where(s => s.Name.Contains(input.FilterText));
            }

            // 统计查询数据总数，用于分页计算总页数
            var count = query.Count();

            // 根据需求进行排序，进行分页逻辑计算
            query = query
                .OrderBy(input.Sorting)
                .Skip((input.CurrentPage - 1) * input.MaxResultCount)
                .Take(input.MaxResultCount);

            // 将查询结果转换为List集合，加载到内存中
            var models = await query
                .Include(a => a.Administrator)
                .AsNoTracking()
                .ToListAsync();

            var dtos = new PagedResultDto<Department>
            {
                TotalCount = count,
                CurrentPage = input.CurrentPage,
                MaxResultCount = input.MaxResultCount,
                Data = models,
                FilterText = input.FilterText,
                Sorting = input.Sorting
            };

            return dtos;
        }
    }
}
