using Microsoft.EntityFrameworkCore;
using SkylinePlanManagementSystem.Application.Dtos;
using SkylinePlanManagementSystem.Application.Projects.Dtos;
using SkylinePlanManagementSystem.DataRepositories;
using SkylinePlanManagementSystem.Infrastructure.Repositories;
using SkylinePlanManagementSystem.Models;
using System.Linq.Dynamic.Core;

namespace SkylinePlanManagementSystem.Application.Projects
{
    public class ProjectService:IProjectService
    {
        private readonly IRepository<Project, int> _projectRepository;

        public ProjectService(IRepository<Project, int> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<PagedResultDto<Project>> GetPaginatedResult(GetProjectInput input)
        {
            input.Sorting = string.IsNullOrWhiteSpace(input.Sorting) ? "ProjectId" : input.Sorting;
            var query = _projectRepository.GetAll();

            if (!string.IsNullOrEmpty(input.FilterText))
            {
                query = query.Where(s =>
                    s.ProjectName.Contains(input.FilterText) ||
                    (s.Remark != null && s.Remark.Contains(input.FilterText)));
            }

            // 统计查询数据的总数，用于分页计算总页数
            var count = query.Count();

            // 根究需求进行排序，然后进行分页逻辑的计算

            try
            {
                query = query.OrderBy(input.Sorting);
            }
            catch
            {
                // 避免排序字段传错导致页面直接报错，回退到主键排序
                query = query.OrderBy("ProjectId");
                input.Sorting = "ProjectId";
            }

            query = query
                .Skip((input.CurrentPage - 1) * input.MaxResultCount)
                .Take(input.MaxResultCount);

            // 将查询结果转换为List集合，加载到内存中
            var models = await query.AsNoTracking().ToListAsync();
            var dtos = new PagedResultDto<Project>()
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
