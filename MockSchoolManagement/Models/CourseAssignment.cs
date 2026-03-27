namespace MockSchoolManagement.Models
{
    /// <summary>
    /// 课程分配设置
    /// </summary>
    public class CourseAssignment
    {
        public int TeacherId { get; set; }

        public int CourseId { get; set; }

        public Teacher Teacher { get; set; }

        public Course Course { get; set; }
    }
}
