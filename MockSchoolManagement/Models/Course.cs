using System.ComponentModel.DataAnnotations.Schema;

namespace MockSchoolManagement.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; }
    }
}
