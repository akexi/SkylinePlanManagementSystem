using SkylinePlanManagementSystem.Models.EnumTypes;
using System.ComponentModel.DataAnnotations;

namespace SkylinePlanManagementSystem.ViewModels.ProjectPlan
{
    public class ProjectSubNodeEditViewModel
    {
        public int ProjectSubNodeId { get; set; }
        public int ProjectId { get; set; }
        public int ProjectNodeId { get; set; }

        [Display(Name = "子节点名称")]
        [Required(ErrorMessage = "请输入子节点名称")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "子节点名称长度应在2-100个字符之间")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "子节点明细")]
        [StringLength(1000, ErrorMessage = "子节点明细最多1000个字符")]
        public string? Detail { get; set; }

        [Display(Name = "计划开始时间")]
        [DataType(DataType.Date)]
        public DateTime? PlanStartTime { get; set; }

        [Display(Name = "计划完成时间")]
        [DataType(DataType.Date)]
        public DateTime? PlanEndTime { get; set; }

        [Display(Name = "完成情况")]
        public SubNodeProgressStatus ProgressStatus { get; set; } = SubNodeProgressStatus.未开始;

        [Display(Name = "备注")]
        [StringLength(200, ErrorMessage = "备注最多200个字符")]
        public string? Remark { get; set; }
    }
}
