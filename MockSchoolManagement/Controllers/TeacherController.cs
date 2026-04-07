using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.Application.Courses;
using MockSchoolManagement.Application.Teachers;
using MockSchoolManagement.Application.Teachers.Dtos;
using MockSchoolManagement.ViewModels.Teachers;

namespace MockSchoolManagement.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        public async Task<IActionResult> Index(GetTeacherInput input)
        {
            var models = await _teacherService.GetPagedTeacherList(input);
            var dto = new TeacherListViewModel();

            if(input.Id != null)
            {
                // 查询教师教授的课程列表
                var teacher = models.Data.FirstOrDefault(a => a.Id == input.Id.Value);

                if(teacher != null)
                {
                    dto.Courses = teacher.CourseAssignments.Select(a => a.Course).ToList();
                }

                dto.SelectedId = input.Id.Value;
            }

            if (input.CourseId.HasValue)
            {
                // 查询该课程下有多少学生选修了该课程
                var course = dto.Courses.FirstOrDefault(a => a.CourseId == input.CourseId.Value);

                if(course != null)
                {
                    dto.StudentCourses = course.StudentCourses.ToList();
                }

                dto.SelectedCourseId = input.CourseId.Value;
            }

            dto.Teachers = models;
            return View(dto);
        }
    }
}
