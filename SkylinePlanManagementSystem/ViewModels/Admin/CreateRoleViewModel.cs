using System.ComponentModel.DataAnnotations;

namespace SkylinePlanManagementSystem.ViewModels.Admin
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "角色")]
        public string RoleName { get; set; }
    }
}
