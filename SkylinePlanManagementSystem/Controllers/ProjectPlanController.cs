using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
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
        private readonly IRepository<ProjectSubNode, int> _projectSubNodeRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Department, int> _departmentRepository;

        // 使用构造函数注入的方式注入
        public ProjectPlanController(IRepository<Project, int> projectRepository, 
            IProjectService projectService,
            IRepository<ProjectNode, int> projectNodeRepository,
            IRepository<ProjectSubNode, int> projectSubNodeRepository,
            UserManager<ApplicationUser> userManager,
            IRepository<Department, int> departmentRepository)
        {
            _projectRepository = projectRepository;
            _projectService = projectService;
            _projectNodeRepository = projectNodeRepository;
            _projectSubNodeRepository = projectSubNodeRepository;
            _userManager = userManager;
            _departmentRepository = departmentRepository;
        }

        private bool IsAjaxRequest()
        {
            return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
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
            var project = await _projectRepository.GetAll().FirstOrDefaultAsync(a => a.ProjectId == id);
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

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Project(int id)
        {
            var project = await _projectRepository.GetAll()
                .Include(p => p.Nodes)
                .ThenInclude(n => n.Department)
                .Include(p => p.Nodes)
                .ThenInclude(n => n.SubNodes)
                .ThenInclude(sn => sn.Department)
                .FirstOrDefaultAsync(a => a.ProjectId == id);

            if(project == null)
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
            var user = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUserDepartmentId = user?.DepartmentId;
            ViewBag.CurrentUserDepartmentName = user?.DepartmentId.HasValue == true
                ? (await _departmentRepository.FirstOrDefaultAsync(d => d.DepartmentId == user.DepartmentId.Value)).Name
                : null;

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
                if (IsAjaxRequest()) return Json(new { success = false, message = "子节点数据不完整，请检查后重试" });
                return RedirectToAction(nameof(Project), new { id = input.ProjectId });
            }

            var project = await _projectRepository.GetAll().Include(p => p.Nodes).FirstOrDefaultAsync(a => a.ProjectId == input.ProjectId);
            if (project == null)
            {
                ViewBag.ErrorMessage = $"项目编号为{input.ProjectId}的信息不存在，请重试";
                return View("NotFound");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user?.DepartmentId == null)
            {
                TempData["ProjectNodeError"] = "当前用户未绑定部门，无权新增一级节点";
                return RedirectToAction(nameof(Project), new { id = input.ProjectId });
            }

            var node = new ProjectNode
            {
                ProjectId = input.ProjectId,
                Title = input.Title,
                PlanTime = input.PlanTime,
                DepartmentId = user.DepartmentId,
            };

            project.Nodes ??= new List<ProjectNode>();
            project.Nodes.Add(node);
            await _projectRepository.UpdateAsync(project);

            return RedirectToAction(nameof(Project), new { id = input.ProjectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNode(ProjectNodeEditViewModel input)
        {
            if (!ModelState.IsValid)
            {
                TempData["ProjectNodeError"] = "节点数据不完整，请检查后重试";
                return RedirectToAction(nameof(Project), new { id = input.ProjectId });
            }

            var node = await _projectNodeRepository.FirstOrDefaultAsync(n => n.ProjectNodeId == input.ProjectNodeId && n.ProjectId == input.ProjectId);
            if (node == null)
            {
                TempData["ProjectNodeError"] = "未找到对应节点，可能已被删除";
                return RedirectToAction(nameof(Project), new { id = input.ProjectId });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user?.DepartmentId == null || node.DepartmentId != user.DepartmentId)
            {
                TempData["ProjectNodeError"] = "仅可修改本部门的一级节点";
                return RedirectToAction(nameof(Project), new { id = input.ProjectId });
            }

            node.Title = input.Title;
            node.PlanTime = input.PlanTime;
            await _projectNodeRepository.UpdateAsync(node);

            return RedirectToAction(nameof(Project), new { id = input.ProjectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNode(int projectId, int projectNodeId)
        {
            var node = await _projectNodeRepository.FirstOrDefaultAsync(n => n.ProjectNodeId == projectNodeId && n.ProjectId == projectId);
            if (node == null)
            {
                TempData["ProjectNodeError"] = "未找到对应节点，可能已被删除";
                return RedirectToAction(nameof(Project), new { id = projectId });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user?.DepartmentId == null || node.DepartmentId != user.DepartmentId)
            {
                TempData["ProjectNodeError"] = "仅可删除本部门的一级节点";
                return RedirectToAction(nameof(Project), new { id = projectId });
            }

            var hasSubNodes = await _projectSubNodeRepository.GetAll().AnyAsync(sn => sn.ProjectNodeId == node.ProjectNodeId);
            if (hasSubNodes)
            {
                TempData["ProjectNodeError"] = "该一级节点下存在子节点，请先删除子节点后再删除一级节点";
                return RedirectToAction(nameof(Project), new { id = projectId });
            }

            await _projectNodeRepository.DeleteAsync(node);
            return RedirectToAction(nameof(Project), new { id = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSubNode(ProjectSubNodeCreateViewModel input)
        {
            if (!ModelState.IsValid)
            {
                if (IsAjaxRequest()) return Json(new { success = false, message = "子节点数据不完整，请检查后重试" });
                TempData["ProjectNodeError"] = "子节点数据不完整，请检查后重试";
                return RedirectToAction(nameof(Project), new { id = input.ProjectId });
            }

            var parentNode = await _projectNodeRepository.FirstOrDefaultAsync(n => n.ProjectNodeId == input.ProjectNodeId && n.ProjectId == input.ProjectId);
            if(parentNode == null)
            {
                if (IsAjaxRequest()) return Json(new { success = false, message = "未找到对应的一级节点" });
                TempData["ProjectNodeError"] = "未找到对应的一级节点";
                return RedirectToAction(nameof(Project), new { id = input.ProjectId });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user?.DepartmentId == null || parentNode.DepartmentId != user.DepartmentId)
            {
                if (IsAjaxRequest()) return Json(new { success = false, message = "仅可为本部门的一级节点新增子节点" });
                TempData["ProjectNodeError"] = "仅可为本部门的一级节点新增子节点";
                return RedirectToAction(nameof(Project), new {id = input.ProjectId});
            }

            var subNode = new ProjectSubNode
            {
                ProjectNodeId = input.ProjectNodeId,
                Title = input.Title,
                PlanTime = input.PlanTime,
                DepartmentId = user.DepartmentId,
            };
            await _projectSubNodeRepository.InsertAsync(subNode);

            if (IsAjaxRequest()) return Json(new { success = true, subNodeId = subNode.ProjectSubNodeId, title = subNode.Title, planTime = subNode.PlanTime?.ToString("yyyy-MM-dd") });
            return RedirectToAction(nameof(Project), new { id = input.ProjectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSubNode(ProjectSubNodeEditViewModel input)
        {
            if (!ModelState.IsValid)
            {
                TempData["ProjectNodeError"] = "子节点数据不完整，请检查后重试";
                return RedirectToAction(nameof(Project), new { id = input.ProjectId });
            }

            var subNode = await _projectSubNodeRepository.FirstOrDefaultAsync(sn => sn.ProjectSubNodeId == input.ProjectSubNodeId && sn.ProjectNodeId == input.ProjectNodeId);
            if (subNode == null)
            {
                if (IsAjaxRequest()) return Json(new { success = false, message = "未找到对应子节点，可能已被删除" });
                TempData["ProjectNodeError"] = "未找到对应子节点，可能已被删除";
                return RedirectToAction(nameof(Project), new { id = input.ProjectId });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user?.DepartmentId == null || subNode.DepartmentId != user.DepartmentId)
            {
                if (IsAjaxRequest()) return Json(new { success = false, message = "仅可修改本部门的子节点" });
                TempData["ProjectNodeError"] = "仅可修改本部门的子节点";
                return RedirectToAction(nameof(Project), new { id = input.ProjectId });
            }

            subNode.Title = input.Title;
            subNode.PlanTime = input.PlanTime;
            await _projectSubNodeRepository.UpdateAsync(subNode);

            if (IsAjaxRequest()) return Json(new { success = true, title = subNode.Title, planTime = subNode.PlanTime?.ToString("yyyy-MM-dd") });
            return RedirectToAction(nameof(Project), new { id = input.ProjectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubNode(int projectId, int projectNodeId, int projectSubNodeId)
        {
            var subNode = await _projectSubNodeRepository.FirstOrDefaultAsync(sn => sn.ProjectSubNodeId == projectSubNodeId && sn.ProjectNodeId == projectNodeId);
            if (subNode == null)
            {
                if (IsAjaxRequest()) return Json(new { success = false, message = "未找到对应子节点，可能已被删除" });
                TempData["ProjectNodeError"] = "未找到对应子节点，可能已被删除";
                return RedirectToAction(nameof(Project), new { id = projectId });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user?.DepartmentId == null || subNode.DepartmentId != user.DepartmentId)
            {
                if (IsAjaxRequest()) return Json(new { success = false, message = "仅可删除本部门的子节点" });
                TempData["ProjectNodeError"] = "仅可删除本部门的子节点";
                return RedirectToAction(nameof(Project), new { id = projectId });
            }

            await _projectSubNodeRepository.DeleteAsync(subNode);
            if (IsAjaxRequest()) return Json(new { success = true });
            return RedirectToAction(nameof(Project), new { id = projectId });
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
            if(project.Nodes != null && project.Nodes.Any())
            {
                TempData["ProjectError"] = $"项目【{project.ProjectName}】下存在一级节点，请先删除节点后再删除项目。";
                return RedirectToAction(nameof(Index));
            }

            await _projectRepository.DeleteAsync(project);
            TempData["ProjectSuccess"] = $"项目【{project.ProjectName}】已删除";
            return RedirectToAction(nameof(Index));
        }


    }
}
