namespace SkylinePlanManagementSystem.ViewModels.ProjectPlan
{
    public class ProjectNodeExportRowViewModel
    {
        public string ProjectName { get; set; } = string.Empty;
        public string? NodeTitle { get; set; }
        public string? SubNodeTitle { get; set; }
        public DateTime? PlanStartTime { get; set; }
        public DateTime? PlanEndTime { get; set; }
    }
}
