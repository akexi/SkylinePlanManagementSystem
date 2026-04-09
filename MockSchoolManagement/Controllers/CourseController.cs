using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Application.Courses;
using MockSchoolManagement.Application.Courses.Dtos;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Infrastructure;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels.Courses;

namespace MockSchoolManagement.Controllers
{
    public class CourseController : Controller
    {
        private readonly IRepository<Course, int> _courseRepository;
        private readonly IRepository<Department, int> _departmentRepository;
        private readonly ICourseService _courseService;

        public CourseController(IRepository<Course, int> courseRepository, IRepository<Department, int> departmentRepository, ICourseService courseService)
        {
            _courseRepository = courseRepository;
            _departmentRepository = departmentRepository;
            _courseService = courseService;
        }

        // 不写[HttpGet]，默认就是GET请求
        public async Task<ActionResult> Index(GetCourseInput input)
        {
            var models = await _courseService.GetPaginatedResult(input);
            return View(models);
        }

        #region 添加课程

        [HttpGet]
        public ActionResult Create()
        {
            var dtos = DepartmentsDropDownList();
            CourseCreateViewModel courseCreateViewModel = new CourseCreateViewModel
            {
                DepartmentList = dtos
            };

            // 将DepartmentsDropDownList方法的SelectList返回值添加到CourseCreateViewModel中，传递到视图中
            return View(courseCreateViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CourseCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                Course course = new Course
                {
                    CourseId = input.CourseId,
                    Title = input.Title,
                    Credits = input.Credits,
                    DepartmentId = input.DepartmentId
                };

                await _courseRepository.InsertAsync(course);

                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        #endregion 添加课程

        /// <summary>
        /// 学院的下拉列表
        /// </summary>
        /// <param name="selectedDepartmentId"></param>
        private SelectList DepartmentsDropDownList(object selectedDepartment = null)
        {
            var models = _departmentRepository.GetAll().OrderBy(a => a.Name).AsNoTracking().ToList();
            var dtos = new SelectList(models, "DepartmentId", "Name", selectedDepartment);

            return dtos;
        }




    }
}
