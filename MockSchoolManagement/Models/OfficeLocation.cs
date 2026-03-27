using System.ComponentModel.DataAnnotations;

namespace MockSchoolManagement.Models
{
    /// <summary>
    /// 办公室地点
    /// </summary>
    public class OfficeLocation
    {
        [Key]
        public int TeacherId { get; set; }

        [StringLength(50)]
        [Display(Name = "办公室位置")]
        public string Location { get; set; }

        public Teacher Teacher { get; set; }
    }
}
