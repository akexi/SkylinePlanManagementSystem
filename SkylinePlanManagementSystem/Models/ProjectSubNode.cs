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

        public DateTime? PlanTime { get; set; }

        [ForeignKey(nameof(ProjectNode))]
        public int ProjectNodeId { get; set; }
        public ProjectNode ProjectNode { get; set; } = null!;

        [ForeignKey(nameof(Department))]
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}
