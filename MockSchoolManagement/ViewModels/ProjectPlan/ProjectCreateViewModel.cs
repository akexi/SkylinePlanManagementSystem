using SkylinePlanManagementSystem.Models.EnumTypes;
using System.ComponentModel.DataAnnotations;

namespace SkylinePlanManagementSystem.ViewModels.ProjectPlan
{
    public class ProjectCreateViewModel
    {
        [Display(Name = "项目名称")]
        [Required(ErrorMessage = "请输入项目名称")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "项目名称长度应在2-100个字符之间")]
        public string ProjectName { get; set; } = string.Empty;

        [Display(Name = "备注")]
        [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
        public string? Remark { get; set; }

        [Display(Name = "状态")]
        [Required(ErrorMessage = "请选择项目状态")]
        public ProjectStatus Status { get; set; } = ProjectStatus.未开始;
    }
}
