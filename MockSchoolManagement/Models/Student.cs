namespace MockSchoolManagement.Models
{
    using MockSchoolManagement.Models.EnumTypes;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 学生模型
    /// </summary>
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 主修科目
        /// </summary>
        public MajorEnum? Major {  get; set; }

        public string Email { get; set; }

        /// <summary>
        /// 学生头像路径
        /// </summary>
        public string PhotoPath { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }
    }
}
