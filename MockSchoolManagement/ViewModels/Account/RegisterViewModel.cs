using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.CustomerMiddlewares.Utils;
using System.ComponentModel.DataAnnotations;

namespace MockSchoolManagement.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "邮箱地址")]
        [Remote(action: "IsEmailInUse", controller: "Account")] // 远程验证,详细见AccountController.cs中的IsEmailInUse方法
        // [ValidEmailDomain(allowedDomain: "52abp.com", ErrorMessage = "邮箱域名必须是 52abp.com")]  // 自定义验证特性,详细见CustomerMiddlewares/Utils/ValidEmail-DomainAttribute.cs
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配，请重新输入.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "城市")]
        public string City { get; set; }
    }
}
