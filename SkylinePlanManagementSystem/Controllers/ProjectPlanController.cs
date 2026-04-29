using Microsoft.AspNetCore.Mvc;
using SkylinePlanManagementSystem.Application.Projects;
using SkylinePlanManagementSystem.Application.Projects.Dtos;
using SkylinePlanManagementSystem.Infrastructure.Repositories;
using SkylinePlanManagementSystem.Models;
using SkylinePlanManagementSystem.ViewModels.ProjectPlan;
using Microsoft.EntityFrameworkCore;

namespace SkylinePlanManagementSystem.Controllers
{
    public class ProjectPlanController : Controller
    {
        private readonly IRepository<Project, int> _projectRepository;  // 项目仓储接口
        private readonly IProjectService _projectService;
        private readonly IRepository<ProjectNode, int> _projectNodeRepository;

        // 使用构造函数注入的方式注入
        public ProjectPlanController(IRepository<Project, int> projectRepository, 
            IProjectService projectService,
            IRepository<ProjectNode, int> projectNodeRepository)
        {
            _projectRepository = projectRepository;
            _projectService = projectService;
            _projectNodeRepository = projectNodeRepository;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectCreateViewModel input)
        {
            if (input.StartTime.HasValue && input.EndTime.HasValue && input.EndTime < input.StartTime)
            {
                ModelState.AddModelError(nameof(input.EndTime), "结束时间不能早于开始时间");
            }

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var entity = new Project
            {
                ProjectName = input.ProjectName,
                Remark = input.Remark,
                StartTime = input.StartTime,
                EndTime = input.EndTime,
                Status = input.Status
            };

            await _projectRepository.InsertAsync(entity);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _projectRepository.GetAll().Include(p => p.Nodes).FirstOrDefaultAsync(a => a.ProjectId == id);
            if (project == null)
            {
                ViewBag.ErrorMessage = $"项目编号为{id}的信息不存在，请重试";
                return View("NotFound");
            }

            var model = new ProjectCreateViewModel
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                Remark = project.Remark,
                StartTime = project.StartTime,
                EndTime = project.EndTime,
                Status = project.Status
            };

            ViewBag.ProjectNodes = project.Nodes?.ToList() ?? new List<ProjectNode>();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectCreateViewModel input)
        {
            if (input.StartTime.HasValue && input.EndTime.HasValue && input.EndTime < input.StartTime)
            {
                ModelState.AddModelError(nameof(input.EndTime), "结束时间不能早于开始时间");
            }

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var project = await _projectRepository.GetAll().Include(p => p.Nodes).FirstOrDefaultAsync(a => a.ProjectId == input.ProjectId);
            if (project == null)
            {
                ViewBag.ErrorMessage = $"项目编号为{input.ProjectId}的信息不存在，请重试";
                return View("NotFound");
            }

            project.ProjectName = input.ProjectName;
            project.Remark = input.Remark;
            project.StartTime = input.StartTime;
            project.EndTime = input.EndTime;
            project.Status = input.Status;

            await _projectRepository.UpdateAsync(project);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNode(ProjectNodeCreateViewModel input)
        {
            if (!ModelState.IsValid)
            {
                TempData["ProjectNodeError"] = "节点数据不完整，请检查后重试";
                return RedirectToAction(nameof(Edit), new { id = input.ProjectId });
            }

            var project = await _projectRepository.GetAll().Include(p => p.Nodes).FirstOrDefaultAsync(a => a.ProjectId == input.ProjectId);
            if (project == null)
            {
                ViewBag.ErrorMessage = $"项目编号为{input.ProjectId}的信息不存在，请重试";
                return View("NotFound");
            }

            var node = new ProjectNode
            {
                ProjectId = input.ProjectId,
                Title = input.Title,
                PlanTime = input.PlanTime
            };

            project.Nodes ??= new List<ProjectNode>();
            project.Nodes.Add(node);
            await _projectRepository.UpdateAsync(project);

            return RedirectToAction(nameof(Edit), new { id = input.ProjectId });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNode(ProjectNodeEditViewModel input)
        {
            if (!ModelState.IsValid)
            {
                TempData["ProjectNodeError"] = "节点数据不完整，请检查后重试";
                return RedirectToAction(nameof(Edit), new { id = input.ProjectId });
            }

            var node = await _projectNodeRepository.FirstOrDefaultAsync(n => n.ProjectNodeId == input.ProjectNodeId && n.ProjectId == input.ProjectId);
            if (node == null)
            {
                TempData["ProjectNodeError"] = "未找到对应节点，可能已被删除";
                return RedirectToAction(nameof(Edit), new { id = input.ProjectId });
            }

            node.Title = input.Title;
            node.PlanTime = input.PlanTime;
            await _projectNodeRepository.UpdateAsync(node);

            return RedirectToAction(nameof(Edit), new { id = input.ProjectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNode(int projectId, int projectNodeId)
        {
            var node = await _projectNodeRepository.FirstOrDefaultAsync(n => n.ProjectNodeId == projectNodeId && n.ProjectId == projectId);
            if (node == null)
            {
                TempData["ProjectNodeError"] = "未找到对应节点，可能已被删除";
                return RedirectToAction(nameof(Edit), new { id = projectId });
            }

            await _projectNodeRepository.DeleteAsync(node);
            return RedirectToAction(nameof(Edit), new { id = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _projectRepository.GetAll().Include(p => p.Nodes).FirstOrDefaultAsync(a => a.ProjectId == id);
            if (project == null)
            {
                ViewBag.ErrorMessage = $"项目编号为{id}的信息不存在，请重试";
                return View("NotFound");
            }

            await _projectRepository.DeleteAsync(project);
            return RedirectToAction(nameof(Index));
        }


    }
}
