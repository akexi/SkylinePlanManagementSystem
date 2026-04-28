using System.ComponentModel.DataAnnotations;

namespace SkylinePlanManagementSystem.ViewModels.Admin
{
    public class AdminResetPasswordViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Display(Name = "用户名")]
        public string? UserName { get; set; }

        [Display(Name = "邮箱")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("NewPassword", ErrorMessage = "密码与确认密码不一致，请重新输入")]
        public string ConfirmPassword { get; set; }
    }
}
