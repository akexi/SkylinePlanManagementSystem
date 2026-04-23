using System.ComponentModel.DataAnnotations;

namespace SkylinePlanManagementSystem.ViewModels.Account
{
    public class AddPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="新密码:")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认新密码:")]
        [Compare("NewPassword", ErrorMessage = "新密码和确认新密码不匹配，请重新输入。")]
        public string ConfirmPassword { get; set; }
    }
}
