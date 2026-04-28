using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SkylinePlanManagementSystem.Application.Departments;
using SkylinePlanManagementSystem.Application.Departments.Dtos;
using SkylinePlanManagementSystem.Infrastructure;
using SkylinePlanManagementSystem.Infrastructure.Repositories;
using SkylinePlanManagementSystem.Models;
using SkylinePlanManagementSystem.ViewModels.Department;

namespace SkylinePlanManagementSystem.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IRepository<Department, int> _departmentRepository;
        private readonly IDepartmentsService _departmentsService;
        private readonly AppDbContext _dbcontext;
        private readonly UserManager<ApplicationUser> _userManager;

        public DepartmentsController(
            IRepository<Department, int> departmentRepository,
            IDepartmentsService departmentsService,
            AppDbContext dbcontext,
            UserManager<ApplicationUser> userManager)
        {
            _departmentRepository = departmentRepository;
            _departmentsService = departmentsService;
            _dbcontext = dbcontext;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(GetDepartmentInput input)
        {
            var models = await _departmentsService.GetPagedDepartmentsList(input);

            return View(models);
        }

        /// <summary>
        /// 用户的下拉列表
        /// </summary>
        /// <param name="selectedAdministratorId"></param>
        private SelectList AdministratorDropDownList(object selectedAdministratorId = null)
        {
            var models = _userManager.Users
                .OrderBy(a => a.Name)
                .AsNoTracking()
                .ToList();

            return new SelectList(models, "Id", "Name", selectedAdministratorId);
        }

        #region 添加

        [HttpGet]
        public IActionResult Create()
        {
            var dto = new DepartmentCreateViewModel
            {
                AdministratorList = AdministratorDropDownList()
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                Department model = new Department
                {
                    StartDate = input.StartDate,
                    DepartmentId = input.DepartmentId,
                    AdministratorId = input.AdministratorId,
                    Budget = input.Budget,
                    Name = input.Name
                };

                await _departmentRepository.InsertAsync(model);

                return RedirectToAction(nameof(Index));
            }

            input.AdministratorList = AdministratorDropDownList(input.AdministratorId);
            return View(input);
        }

        #endregion 添加

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            string query = "SELECT * FROM Departments WHERE DepartmentId = {0}";
            var model = await _dbcontext.Departments.FromSqlRaw(query, id)
                .Include(a => a.Administrator)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            // 判断学院信息是否存在
            if (model == null)
            {
                ViewBag.ErrorMessage = $"学院ID为{id}的信息不存在，请重试。";
                return View("NotFound");
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _departmentRepository.FirstOrDefaultAsync(a => a.DepartmentId == id);

            if (model == null)
            {
                ViewBag.ErrorMessage = $"学院ID为{id}的信息不存在，请重试。";
                return View("NotFound");
            }

            await _departmentRepository.DeleteAsync(a => a.DepartmentId == id);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _departmentRepository
                .GetAll()
                .Include(a => a.Administrator)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.DepartmentId == id);

            if (model == null)
            {
                ViewBag.ErrorMessage = $"学院ID为{id}的信息不存在，请重试。";
                return View("NotFound");
            }

            var dto = new DepartmentCreateViewModel
            {
                DepartmentId = model.DepartmentId,
                Name = model.Name,
                Budget = model.Budget,
                StartDate = model.StartDate,
                AdministratorId = model.AdministratorId,
                Administrator = model.Administrator,
                RowVersion = model.RowVersion,
                AdministratorList = AdministratorDropDownList(model.AdministratorId)
            };

            return View(dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(DepartmentCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                var model = await _departmentRepository
                    .GetAll()
                    .Include(a => a.Administrator)
                    .FirstOrDefaultAsync(a => a.DepartmentId == input.DepartmentId);

                if (model == null)
                {
                    ViewBag.ErrorMessage = $"学院ID为{input.DepartmentId}的信息不存在，请重试。";
                    return View("NotFound");
                }

                model.DepartmentId = input.DepartmentId;
                model.Name = input.Name;
                model.Budget = input.Budget;
                model.StartDate = input.StartDate;
                model.AdministratorId = input.AdministratorId;

                _dbcontext.Entry(model).Property("RowVersion").OriginalValue = input.RowVersion;

                try
                {
                    await _departmentRepository.UpdateAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException concurrencyEx)
                {
                    var exceptionEntry = concurrencyEx.Entries.Single();
                    var clientValues = (Department)exceptionEntry.Entity;

                    // 从数据库中获取Department实体中的RowVersion值，然后将input.RowVersion赋值到OriginalValue属性中，以便在下一次更新时进行比较
                    _dbcontext.Entry(model).Property("RowVersion").OriginalValue = input.RowVersion;

                    try
                    {
                        // UpdateAsync()方法执行SaveChanges()方法时，如检测到并发冲突，则会抛出DbUpdateConcurrencyException异常
                        await _departmentRepository.UpdateAsync(model);
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException innerConcurrencyEx)
                    {
                        // 触发异常后，获取异常实体
                        var innerExceptionEntry = innerConcurrencyEx.Entries.Single();
                        var innerClientValues = (Department)innerExceptionEntry.Entity;

                        // 从数据库中获取该异常实体信息
                        var databaseEntry = innerExceptionEntry.GetDatabaseValues();
                        if (databaseEntry == null)
                        {
                            // 如果实体null，则表示该数据已经被删除了
                            ModelState.AddModelError(string.Empty, "无法保存更改。该数据已经被删除了。");
                        }
                        else
                        {
                            // 将异常实体中的错误信息精确到具体字段并传递到前端
                            var databaseValues = (Department)databaseEntry.ToObject();

                            if (databaseValues.Name != innerClientValues.Name)
                                ModelState.AddModelError("Name", $"当前值: {databaseValues.Name}");
                            if (databaseValues.Budget != innerClientValues.Budget)
                                ModelState.AddModelError("Budget", $"当前值: {databaseValues.Budget}");
                            if (databaseValues.StartDate != innerClientValues.StartDate)
                                ModelState.AddModelError("StartDate", $"当前值: {databaseValues.StartDate}");
                            if (databaseValues.AdministratorId != innerClientValues.AdministratorId)
                            {
                                var adminUser = databaseValues.AdministratorId == null
                                    ? null
                                    : await _userManager.FindByIdAsync(databaseValues.AdministratorId);

                                ModelState.AddModelError("AdministratorId", $"当前值：{adminUser?.Name ?? "未设置"}");
                            }

                            ModelState.AddModelError("", "数据已被修改！编辑操作已取消。最新值已显示在各个字段中，请再次尝试提交。");
                            input.RowVersion = databaseValues.RowVersion;

                            // 初始化用户的下拉列表
                            input.AdministratorList = AdministratorDropDownList(input.AdministratorId);
                            ModelState.Remove("RowVersion");
                        }
                    }
                }

                return View(input);
            }

            input.AdministratorList = AdministratorDropDownList(input.AdministratorId);
            return View(input);
        }



    }
}
