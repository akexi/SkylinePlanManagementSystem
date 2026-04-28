using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SkylinePlanManagementSystem.ViewModels.Admin
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Claims = new List<Claim>();
            Roles = new List<string>();
        }

        public string Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "用户名长度应在3-30个字符之间")]
        [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "用户名仅支持字母、数字和下划线")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "请输入姓名")]
        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Required(ErrorMessage = "请输入手机号")]
        [Display(Name = "手机号")]
        [Phone(ErrorMessage = "请输入有效的手机号")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "请选择所属部门")]
        [Display(Name = "所属部门")]
        public int? DepartmentId { get; set; }

        [Display(Name = "账户有效")]
        public bool IsActive { get; set; } = true;

        public SelectList? DepartmentList { get; set; }

        public IList<Claim> Claims {  get; set; }

        public IList<string> Roles { get; set; }
    }
}
