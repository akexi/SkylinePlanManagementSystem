using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SkylinePlanManagementSystem.ViewModels.Courses
{
    public class CourseCreateViewModel
    {
        [Display(Name = "课程编号")]
        public int CourseId { get; set; }

        [Display(Name = "课程名称")]
        public string Title { get; set; }

        [Display(Name = "学分")]
        public int Credits { get; set; }

        public int DepartmentId { get; set; }

        [Display(Name = "学院")]
        public SelectList? DepartmentList { get; set; }
    }
}
