using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkylinePlanManagementSystem.Models
{
    public class ProjectNode
    {
        [Key]
        public int ProjectNodeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        public DateTime? PlanTime { get; set; }

        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
