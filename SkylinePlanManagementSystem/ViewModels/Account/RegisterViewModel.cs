using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SkylinePlanManagementSystem.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "邮箱地址")]
        [Remote(action: "IsEmailInUse", controller: "Account")] // 远程验证,详细见AccountController.cs中的IsEmailInUse方法
        // [ValidEmailDomain(allowedDomain: "52abp.com", ErrorMessage = "邮箱域名必须是 52abp.com")]  // 自定义验证特性,详细见CustomerMiddlewares/Utils/ValidEmail-DomainAttribute.cs
        public string Email { get; set; }

        [Required(ErrorMessage = "请输入用户名")]
        [Display(Name = "用户名")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "用户名长度应在3-30个字符之间")]
        [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "用户名仅支持字母、数字和下划线")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "请输入姓名")]
        [Display(Name = "姓名")]
        [StringLength(50, ErrorMessage = "姓名长度不能超过50个字符")]
        public string Name { get; set; }

        [Required(ErrorMessage = "请输入手机号")]
        [Display(Name = "手机号")]
        [Phone(ErrorMessage = "请输入有效的手机号")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "请选择所属部门")]
        [Display(Name = "所属部门")]
        public int? DepartmentId { get; set; }

        public SelectList? DepartmentList { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配，请重新输入.")]
        public string ConfirmPassword { get; set; }

    }
}
