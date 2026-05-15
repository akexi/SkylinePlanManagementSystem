namespace SkylinePlanManagementSystem.ViewModels.ProjectPlan
{
    public class ProjectNodeExportPageViewModel
    {
        public ProjectNodeExportQueryViewModel Query { get; set; } = new();
        public List<ProjectNodeExportRowViewModel> Rows { get; set; } = new();
    }
}
