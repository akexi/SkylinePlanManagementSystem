namespace MockSchoolManagement.Models
{
    using MockSchoolManagement.Models.EnumTypes;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 学生模型
    /// </summary>
    public class Student: Person
    {
        /// <summary>
        /// 主修科目
        /// </summary>
        public MajorEnum? Major {  get; set; }

        /// <summary>
        /// 学生头像路径
        /// </summary>
        public string PhotoPath { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        /// <summary>
        /// 入学时间
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EnrollmentDate { get; set; }

        // 导航属性
        public ICollection<StudentCourse> StudentCourses { get; set; }
    }
}
