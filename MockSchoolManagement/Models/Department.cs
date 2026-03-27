using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MockSchoolManagement.Models
{
    /// <summary>
    /// 学院
    /// </summary>
    public class Department
    {
        public int DepartmentId { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// 预算
        /// </summary>
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Budget { get; set; }

        /// <summary>
        /// 成立时间
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{ 0:yyyy-MM-dd }", ApplyFormatInEditMode = true)]
        [Display(Name = "成立时间")]
        public DateTime StartDate { get; set; }

        public int? TeacherId { get; set; }

        /// <summary>
        /// 学院主任
        /// </summary>
        public Teacher Administrator { get; set; }

        public ICollection<Course> Courses { get; set; }
    }
}
