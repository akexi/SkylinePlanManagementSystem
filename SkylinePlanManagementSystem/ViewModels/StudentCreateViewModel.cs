using SkylinePlanManagementSystem.Models.EnumTypes;
using System.ComponentModel.DataAnnotations;

namespace SkylinePlanManagementSystem.ViewModels
{
    public class StudentCreateViewModel
    {
        [Required(ErrorMessage = "请输入姓名"),MaxLength(50,ErrorMessage = "姓名长度不能超过50个字符")]
        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "主修科目")]
        public MajorEnum? Major { get; set; }

        [Display(Name = "电子邮箱")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "邮箱格式不正确，请输入有效的电子邮箱地址")]
        [Required(ErrorMessage = "请输入电子邮箱")]
        public string Email { get; set; }

        [Display(Name = "头像")]
        //public IFormFile Photo { get; set; }
        public List<IFormFile>? Photos { get; set; }

        [Display(Name = "入学时间")]
        public DateTime EnrollmentDate { get; set; }
    }
}
