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
        private readonly IRepository<CourseAssignment, int> _courseAssignmentRepository;
        private readonly ICourseService _courseService;
        private readonly AppDbContext _dbContext;

        public CourseController(IRepository<Course, int> courseRepository, 
            IRepository<Department, int> departmentRepository, 
            IRepository<CourseAssignment, int> courseAssignmentRepository, 
            ICourseService courseService,
            AppDbContext dbContext)
        {
            _courseRepository = courseRepository;
            _departmentRepository = departmentRepository;
            _courseAssignmentRepository = courseAssignmentRepository;
            _courseService = courseService;
            _dbContext = dbContext;
        }

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

        #region 编辑功能

        [HttpGet]
        public IActionResult Edit(int? courseId)
        {
            if (!courseId.HasValue)
            {
                ViewBag.ErrorMessage = $"课程编号为{courseId}的信息不存在，请重试";
                return View("NotFound");
            }

            var course = _courseRepository.FirstOrDefault(a => a.CourseId == courseId);

            if (courseId == null)
            {
                ViewBag.ErrorMessage = $"课程编号为{courseId}的信息不存在，请重试";
                return View("NotFound");
            }

            // 将学院列表中选中的值修改为true
            var dtos = DepartmentsDropDownList(course.DepartmentId);
            CourseCreateViewModel courseCreateViewModel = new CourseCreateViewModel
            {
                DepartmentList = dtos,
                CourseId = course.CourseId,
                Credits = course.Credits,
                Title = course.Title,
                DepartmentId = course.DepartmentId
            };

            return View(courseCreateViewModel);
        }

        [HttpPost]
        public IActionResult Edit(CourseCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                var course = _courseRepository.FirstOrDefault(a => a.CourseId == input.CourseId);

                if (course != null)
                {
                    course.CourseId = input.CourseId;
                    course.Credits = input.Credits;
                    course.DepartmentId = input.DepartmentId;
                    course.Title = input.Title;
                    _courseRepository.Update(course);
                    return RedirectToAction(nameof(Index)); // 返回列表页
                }
                else
                {
                    ViewBag.ErrorMessage = $"课程编号为{input.CourseId}的信息不存在，请重试";
                    return View("NotFound");
                }
            }

            return View(input);
        }

        #endregion 编辑功能


        #region 删除功能

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _courseRepository.FirstOrDefaultAsync(a => a.CourseId == id);

            if(model == null)
            {
                ViewBag.ErrorMessage = $"课程编号为{id}的信息不存在，请重试";
                return View("NotFound");
            }

            await _courseAssignmentRepository.DeleteAsync(a => a.CourseId == id);   // 这里暂时有问题，如果课程没有教师分配记录，就会报错，后续再优化
            await _courseRepository.DeleteAsync(a => a.CourseId == id);

            return RedirectToAction(nameof(Index));
        }

        #endregion 删除功能

        [HttpGet]
        public async Task<ViewResult> Details(int courseId)
        {
            var course = await _courseRepository.GetAll().Include(a => a.Department).FirstOrDefaultAsync(a => a.CourseId == courseId);

            if(course == null)
            {
                ViewBag.ErrorMessage = $"课程编号为{courseId}的信息不存在，请重试";
                return View("NotFound");
            }

            return View(course);
        }


        #region 修改课程学分功能

        [HttpGet]
        public IActionResult UpdateCourseCredits()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCourseCredits(int? multiplier)
        {
            if(multiplier != null)
            {
                ViewBag.RowsAffected = await _dbContext.Database.ExecuteSqlRawAsync(
                    "UPDATE Course SET Credits = Credits * {0}", 
                    parameters:multiplier);
            }

            return View();
        }

        #endregion

    }
}
