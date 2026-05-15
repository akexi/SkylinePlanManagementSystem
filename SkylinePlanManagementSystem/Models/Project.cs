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

        // 计算项目完成度的属性，范围从0.0到1.0
        public double CompletionProgress { get; set; } = 0.0;

        public ICollection<ProjectNode> Nodes { get; set; } = new List<ProjectNode>();

    }
}
