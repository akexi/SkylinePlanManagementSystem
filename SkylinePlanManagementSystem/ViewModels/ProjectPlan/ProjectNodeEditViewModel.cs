using System.ComponentModel.DataAnnotations;

namespace SkylinePlanManagementSystem.ViewModels.ProjectPlan
{
    public class ProjectNodeEditViewModel
    {
        [Required]
        public int ProjectNodeId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Display(Name = "节点名称")]
        [Required(ErrorMessage = "请输入节点名称")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "节点名称长度应该在2-100个字符之间")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "计划开始时间")]
        [DataType(DataType.Date)]
        public DateTime? PlanStartTime {  get; set; }

        [Display(Name = "计划完成时间")]
        [DataType(DataType.Date)]
        public DateTime? PlanEndTime { get; set; }

        [Display(Name = "备注")]
        [StringLength(200, ErrorMessage = "备注最多200个字符")]
        public string? Remark { get; set; }
    }
}
