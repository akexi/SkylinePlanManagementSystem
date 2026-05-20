namespace SkylinePlanManagementSystem.ViewModels.ProjectPlan
{
    public class ProjectNodeExportRowViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? NodeTitle { get; set; }
        public string? NodeDepartmentName { get; set; }
        public string? SubNodeTitle { get; set; }
        public string? SubNodeDetail { get; set; }
        public string? SubNodeStatus { get; set; }
        public DateTime? PlanStartTime { get; set; }
        public DateTime? PlanEndTime { get; set; }
    }
}
