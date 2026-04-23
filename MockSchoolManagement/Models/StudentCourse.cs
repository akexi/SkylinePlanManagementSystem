using SkylinePlanManagementSystem.Models.EnumTypes;
using System.ComponentModel.DataAnnotations;

namespace SkylinePlanManagementSystem.Models
{
    public class StudentCourse
    {
        [Key]
        public int StudentCourseId { get; set; }

        public int CourseId { get; set; }

        public int StudentId { get; set; }

        [DisplayFormat(NullDisplayText = "无成绩")]
        public Grade? Grade { get; set; }

        public Course Course { get; set; }

        public Student Student { get; set; }
    }
}
