namespace MockSchoolManagement.ViewModels.Teachers
{
    public class AssignedCourseViewModel
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        /// 课程名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
