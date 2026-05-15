using SkylinePlanManagementSystem.Models.EnumTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkylinePlanManagementSystem.Models
{
    public class ProjectSubNode
    {
        [Key]
        public int ProjectSubNodeId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Detail { get; set; }

        public DateTime? PlanStartTime { get; set; }

        public DateTime? PlanEndTime { get; set; }

        public SubNodeProgressStatus ProgressStatus { get; set; } = SubNodeProgressStatus.未开始;

        public string? Remark { get; set; }

        [ForeignKey(nameof(ProjectNode))]
        public int ProjectNodeId { get; set; }
        public ProjectNode ProjectNode { get; set; } = null!;

        [ForeignKey(nameof(Department))]
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}
