using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SkylinePlanManagementSystem.Application.Projects;
using SkylinePlanManagementSystem.Application.Projects.Dtos;
using SkylinePlanManagementSystem.Infrastructure.Repositories;
using SkylinePlanManagementSystem.Models;
using SkylinePlanManagementSystem.ViewModels.ProjectPlan;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;

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
        public async Task<IActionResult> Project(int id, string tab = "all-nodes")
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
                Status = project.Status,
                CompletionProgress = project.CompletionProgress
            };

            ViewBag.ProjectNodes = project.Nodes?.ToList() ?? new List<ProjectNode>();
            var user = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUserDepartmentId = user?.DepartmentId;
            ViewBag.CurrentUserDepartmentName = user?.DepartmentId.HasValue == true
                ? (await _departmentRepository.FirstOrDefaultAsync(d => d.DepartmentId == user.DepartmentId.Value)).Name
                : null;
            ViewBag.ActiveTab = tab;

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
                PlanStartTime = input.PlanStartTime,
                PlanEndTime = input.PlanEndTime,
                Remark = input.Remark,
                DepartmentId = user.DepartmentId,
            };

            project.Nodes ??= new List<ProjectNode>();
            project.Nodes.Add(node);
            await _projectRepository.UpdateAsync(project);

            await RecalculateProjectProgressAsync(input.ProjectId);

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
            node.PlanStartTime = input.PlanStartTime;
            node.PlanEndTime = input.PlanEndTime;
            node.Remark = input.Remark;
            await _projectNodeRepository.UpdateAsync(node);

            await RecalculateProjectProgressAsync(input.ProjectId);

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

            await RecalculateProjectProgressAsync(projectId);

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
                Detail = input.Detail,
                PlanStartTime = input.PlanStartTime,
                PlanEndTime = input.PlanEndTime,
                ProgressStatus = input.ProgressStatus,
                Remark = input.Remark,
                DepartmentId = user.DepartmentId,
            };
            await _projectSubNodeRepository.InsertAsync(subNode);

            await RecalculateProjectProgressAsync(input.ProjectId);

            var updatedNode = await _projectNodeRepository.FirstOrDefaultAsync(n => n.ProjectNodeId == input.ProjectNodeId);
            var updatedProject = await _projectRepository.FirstOrDefaultAsync(p => p.ProjectId == input.ProjectId);

            if (IsAjaxRequest()) return Json(new { 
                success = true, 
                subNodeId = subNode.ProjectSubNodeId, 
                title = subNode.Title, 
                detail = subNode.Detail, 
                planStartTime = subNode.PlanStartTime?.ToString("yyyy-MM-dd"), 
                planEndTime = subNode.PlanEndTime?.ToString("yyyy-MM-dd"), 
                progressStatus = subNode.ProgressStatus.ToString(), 
                remark = subNode.Remark,
                nodeProgress = updatedNode?.CompletionProgress ?? 0d,
                projectProgress = updatedProject?.CompletionProgress ?? 0d
            });
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
            subNode.Detail = input.Detail;
            subNode.PlanStartTime = input.PlanStartTime;
            subNode.PlanEndTime = input.PlanEndTime;
            subNode.ProgressStatus = input.ProgressStatus;
            subNode.Remark = input.Remark;
            await _projectSubNodeRepository.UpdateAsync(subNode);

            await RecalculateProjectProgressAsync(input.ProjectId);

            var updatedNode = await _projectNodeRepository.FirstOrDefaultAsync(n => n.ProjectNodeId == input.ProjectNodeId);
            var updatedProject = await _projectRepository.FirstOrDefaultAsync(p => p.ProjectId == input.ProjectId);

            if (IsAjaxRequest()) return Json(new { 
                success = true, 
                title = subNode.Title, 
                detail = subNode.Detail, 
                planStartTime = subNode.PlanStartTime?.ToString("yyyy-MM-dd"), 
                planEndTime = subNode.PlanEndTime?.ToString("yyyy-MM-dd"), 
                progressStatus = subNode.ProgressStatus.ToString(), 
                remark = subNode.Remark,
                nodeProgress = updatedNode?.CompletionProgress ?? 0d,
                projectProgress = updatedProject?.CompletionProgress ?? 0d
            });
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

            await RecalculateProjectProgressAsync(projectId);

            var updatedNode = await _projectNodeRepository.FirstOrDefaultAsync(n => n.ProjectNodeId == projectNodeId);
            var updatedProject = await _projectRepository.FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (IsAjaxRequest()) return Json(new { 
                success = true,
                nodeProgress = updatedNode?.CompletionProgress ?? 0d,
                projectProgress = updatedProject?.CompletionProgress ?? 0d
            });
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

        [HttpGet]
        public async Task<IActionResult> NodeExport(ProjectNodeExportQueryViewModel query)
        {
            var rows = await BuildNodeExportQuery(query).ToListAsync();
            var model = new ProjectNodeExportPageViewModel
            {
                Query = query,
                Rows = rows
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ExportNodeExcel(ProjectNodeExportQueryViewModel query)
        {
            var rows = await BuildNodeExportQuery(query).ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("项目节点数据");

            worksheet.Cell(1, 1).Value = "项目名称";
            worksheet.Cell(1, 2).Value = "项目ID";
            worksheet.Cell(1, 3).Value = "一级节点";
            worksheet.Cell(1, 4).Value = "所属部门";
            worksheet.Cell(1, 5).Value = "二级节点";
            worksheet.Cell(1, 6).Value = "明细（三级节点）";
            worksheet.Cell(1, 7).Value = "二级节点状态";
            worksheet.Cell(1, 8).Value = "计划开始时间";
            worksheet.Cell(1, 9).Value = "计划结束时间";

            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                worksheet.Cell(i + 2, 1).Value = row.ProjectName;
                worksheet.Cell(i + 2, 2).Value = row.ProjectId;
                worksheet.Cell(i + 2, 3).Value = row.NodeTitle ?? string.Empty;
                worksheet.Cell(i + 2, 4).Value = row.NodeDepartmentName ?? string.Empty;
                worksheet.Cell(i + 2, 5).Value = row.SubNodeTitle ?? string.Empty;
                worksheet.Cell(i + 2, 6).Value = row.SubNodeDetail ?? string.Empty;
                worksheet.Cell(i + 2, 7).Value = row.SubNodeStatus ?? string.Empty;
                worksheet.Cell(i + 2, 8).Value = row.PlanStartTime?.ToString("yyyy-MM-dd") ?? string.Empty;
                worksheet.Cell(i + 2, 9).Value = row.PlanEndTime?.ToString("yyyy-MM-dd") ?? string.Empty;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"项目节点导出_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private IQueryable<ProjectNodeExportRowViewModel> BuildNodeExportQuery(ProjectNodeExportQueryViewModel query)
        {
            var dataQuery =
                from project in _projectRepository.GetAll()
                from node in project.Nodes.DefaultIfEmpty()
                from subNode in node.SubNodes.DefaultIfEmpty()
                select new ProjectNodeExportRowViewModel
                {
                    ProjectId = project.ProjectId,
                    ProjectName = project.ProjectName,
                    NodeTitle = node != null ? node.Title : null,
                    NodeDepartmentName = node != null && node.Department != null ? node.Department.Name : null,
                    SubNodeTitle = subNode != null ? subNode.Title : null,
                    SubNodeDetail = subNode != null ? subNode.Detail : null,
                    SubNodeStatus = subNode != null ? subNode.ProgressStatus.ToString() : null,
                    PlanStartTime = subNode != null ? subNode.PlanStartTime : node != null ? node.PlanStartTime : project.StartTime,
                    PlanEndTime = subNode != null ? subNode.PlanEndTime : node != null ? node.PlanEndTime : project.EndTime
                };

            if (!string.IsNullOrWhiteSpace(query.ProjectName))
            {
                dataQuery = dataQuery.Where(x => x.ProjectName.Contains(query.ProjectName));
            }

            if (!string.IsNullOrWhiteSpace(query.NodeTitle))
            {
                dataQuery = dataQuery.Where(x => x.NodeTitle != null && x.NodeTitle.Contains(query.NodeTitle));
            }

            if (!string.IsNullOrWhiteSpace(query.SubNodeTitle))
            {
                dataQuery = dataQuery.Where(x => x.SubNodeTitle != null && x.SubNodeTitle.Contains(query.SubNodeTitle));
            }

            if (query.StartDate.HasValue)
            {
                dataQuery = dataQuery.Where(x => x.PlanStartTime.HasValue && x.PlanStartTime.Value.Date >= query.StartDate.Value.Date);
            }

            if (query.EndDate.HasValue)
            {
                dataQuery = dataQuery.Where(x => x.PlanEndTime.HasValue && x.PlanEndTime.Value.Date <= query.EndDate.Value.Date);
            }

            return dataQuery.OrderBy(x => x.ProjectName).ThenBy(x => x.NodeTitle).ThenBy(x => x.SubNodeTitle);
        }

        private async Task RecalculateProjectProgressAsync(int projectId)
        {
            var project = await _projectRepository.GetAll()
                .Include(p => p.Nodes)
                .ThenInclude(n => n.SubNodes)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (project == null) return;

            foreach (var node in project.Nodes)
            {
                var totalSubNodes = node.SubNodes.Count;
                if (totalSubNodes == 0)
                {
                    node.CompletionProgress = 0d;
                }
                else
                {
                    var completedSubNodes = node.SubNodes.Count(sn => sn.ProgressStatus == SkylinePlanManagementSystem.Models.EnumTypes.SubNodeProgressStatus.已完成);
                    node.CompletionProgress = Math.Round((double)completedSubNodes * 100d / totalSubNodes, 2);
                }
                await _projectNodeRepository.UpdateAsync(node);
            }

            var allSubNodes = project.Nodes.SelectMany(n => n.SubNodes).ToList();
            var totalProjectSubNodes = allSubNodes.Count;
            if (totalProjectSubNodes == 0)
            {
                project.CompletionProgress = 0d;
            }
            else
            {
                var completedProjectSubNodes = allSubNodes.Count(sn => sn.ProgressStatus == SkylinePlanManagementSystem.Models.EnumTypes.SubNodeProgressStatus.已完成);
                project.CompletionProgress = Math.Round((double)completedProjectSubNodes * 100d / totalProjectSubNodes, 2);
            }
            await _projectRepository.UpdateAsync(project);
        }


    }
}
