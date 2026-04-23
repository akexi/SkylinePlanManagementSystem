using SkylinePlanManagementSystem.Application.Dtos;
using SkylinePlanManagementSystem.Models;

namespace SkylinePlanManagementSystem.ViewModels.Teachers
{
    public class TeacherListViewModel
    {
        public PagedResultDto<Teacher> Teachers { get; set; }
        public List<Course> Courses { get; set; }
        public List<StudentCourse> StudentCourses { get; set; }

        /// <summary>
        /// 选中的教师Id
        /// </summary>
        public int SelectedId { get; set; }

        /// <summary>
        /// 选中的课程Id
        /// </summary>
        public int SelectedCourseId { get; set; }
    }
}
