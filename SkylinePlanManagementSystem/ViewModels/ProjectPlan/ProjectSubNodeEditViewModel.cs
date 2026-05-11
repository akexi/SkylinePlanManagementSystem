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

        [Display(Name = "计划时间")]
        [DataType(DataType.Date)]
        public DateTime? PlanTime {  get; set; }
    }
}
