using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SkylinePlanManagementSystem.ViewModels.Account
{
    public class UserProfileViewModel
    {
        [Display(Name = "用户 Id")]
        public string Id { get; set; }

        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Display(Name = "电子邮箱")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "请输入姓名")]
        [Display(Name = "姓名")]
        [StringLength(50, ErrorMessage = "姓名长度不能超过50个字符")]
        public string Name { get; set; }

        [Required(ErrorMessage = "请输入手机号")]
        [Display(Name = "手机号")]
        [Phone(ErrorMessage = "请输入有效的手机号")]
        public string PhoneNumber { get; set; }

        [Display(Name = "所属部门")]
        public int? DepartmentId { get; set; }

        // 新增：用于在视图中显示部门名称
        [Display(Name = "所属部门")]
        public string DepartmentName { get; set; }

        public SelectList? DepartmentList { get; set; }
    }
}