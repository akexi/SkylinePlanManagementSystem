using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using SkylinePlanManagementSystem.Application.Departments.Dtos;
using SkylinePlanManagementSystem.Application.Projects;
using SkylinePlanManagementSystem.Application.Projects.Dtos;
using SkylinePlanManagementSystem.DataRepositories;
using SkylinePlanManagementSystem.Infrastructure;
using SkylinePlanManagementSystem.Infrastructure.Repositories;
using SkylinePlanManagementSystem.Models;
using SkylinePlanManagementSystem.ViewModels.ProjectPlan;

namespace SkylinePlanManagementSystem.Controllers
{
    public class ProjectPlanController : Controller
    {
        private readonly IRepository<Project, int> _projectRepository;  // 项目仓储接口
        private readonly IProjectService _projectService;

        // 使用构造函数注入的方式注入
        public ProjectPlanController(IRepository<Project, int> projectRepository, 
            IProjectService projectService)
        {
            _projectRepository = projectRepository;
            _projectService = projectService;
        }

        //[Route("")]
        //[Route("ProjectPlan")]
        //[Route("ProjectPlan/Index")]
        public async Task<IActionResult> Index(GetProjectInput input)
        {
            var models = await _projectService.GetPaginatedResult(input);

            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ProjectCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectCreateViewModel input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var entity = new Project
            {
                ProjectName = input.ProjectName,
                Remark = input.Remark,
                Status = input.Status
            };

            await _projectRepository.InsertAsync(entity);

            return RedirectToAction(nameof(Index));
        }



    }
}
