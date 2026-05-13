using SkylinePlanManagementSystem.Models.EnumTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkylinePlanManagementSystem.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 项目开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 项目结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        public ProjectStatus Status { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        public double CompletionProgress { get; set; }

        public ICollection<ProjectNode> Nodes { get; set; } = new List<ProjectNode>();

    }
}
