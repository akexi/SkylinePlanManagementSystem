using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SkylinePlanManagementSystem.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "请输入用户名")]
        [Display(Name = "用户名")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "用户名长度应在3-30个字符之间")]
        [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "用户名仅支持字母、数字和下划线")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "请输入您的密码")]
        [DataType(DataType.Password)]
        [Display(Name ="密码")]
        public string Password { get; set; }

        [Display(Name = "记住我")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }
    }
}
