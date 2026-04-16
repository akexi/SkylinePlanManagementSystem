using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Application.Departments;
using MockSchoolManagement.Application.Departments.Dtos;
using MockSchoolManagement.Infrastructure;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels.Department;

namespace MockSchoolManagement.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IRepository<Department, int> _departmentRepository;
        private readonly IRepository<Teacher, int> _teacherRepository;
        private readonly IDepartmentsService _departmentsService;
        private readonly AppDbContext _dbcontext;

        public DepartmentsController(
            IRepository<Department, int> departmentRepository, 
            IRepository<Teacher, int> teacherRepository, 
            IDepartmentsService departmentsService, 
            AppDbContext dbcontext)
        {
            _departmentRepository = departmentRepository;
            _teacherRepository = teacherRepository;
            _departmentsService = departmentsService;
            _dbcontext = dbcontext;
        }

        public async Task<IActionResult> Index(GetDepartmentInput input)
        {
            var models = await _departmentsService.GetPagedDepartmentsList(input);

            return View(models);
        }

        /// <summary>
        /// 教师的下拉列表
        /// </summary>
        /// <param name="selectedTeacher"></param>
        private SelectList TeacherDropDownList(object selectedTeacher = null)
        {
            var models = _teacherRepository
                .GetAll()
                .OrderBy(a => a.Name)
                .AsNoTracking()
                .ToList();

            var dtos = new SelectList(models, "Id", "Name", selectedTeacher);

            return dtos;
        }

        #region 添加

        [HttpGet]
        public IActionResult Create()
        {
            var dto = new DepartmentCreateViewModel
            {
                TeacherList = TeacherDropDownList()
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
                    TeacherId = input.TeacherId,
                    Budget = input.Budget,
                    Name = input.Name
                };

                await _departmentRepository.InsertAsync(model);

                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        #endregion 添加

        public async Task<IActionResult> Details(int id)
        {
            // 因为要实现预加载，所以不能直接使用FirstOrDefaultAsync()方法
            var model = await _departmentRepository
                .GetAll()
                .Include(a => a.Administrator)
                .FirstOrDefaultAsync(a => a.DepartmentId == id);

            // 判断学院信息是否存在
            if(model == null)
            {
                ViewBag.ErrorMessage = $"学院ID为{id}的信息不存在，请重试。";
                return View("NotFound");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _departmentRepository.FirstOrDefaultAsync(a => a.DepartmentId == id);

            if(model == null)
            {
                ViewBag.ErrorMessage = $"学院ID为{id}的信息不存在，请重试。";
                return View("NotFound");
            }

            await _departmentRepository.DeleteAsync(a => a.DepartmentId == id);

            return RedirectToAction(nameof(Index));
        }

    }
}
