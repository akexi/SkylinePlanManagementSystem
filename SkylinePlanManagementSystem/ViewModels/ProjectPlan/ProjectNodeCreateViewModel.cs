using System.ComponentModel.DataAnnotations;

namespace SkylinePlanManagementSystem.ViewModels.ProjectPlan
{
    public class ProjectNodeCreateViewModel
    {
        [Required]
        public int ProjectId { get; set; }

        [Display(Name = "节点名称")]
        [Required(ErrorMessage = "请输入节点名称")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "节点名称长度应在2-100个字符之间")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "计划时间")]
        [DataType(DataType.Date)]
        public DateTime? PlanTime { get; set; }
    }
}
