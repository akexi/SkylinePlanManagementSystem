using SkylinePlanManagementSystem.Models.EnumTypes;

namespace SkylinePlanManagementSystem.ViewModels
{
    public class NewIndexViewModel
    {
        public string UserDisplayName { get; set; } = "用户";
        public int TotalProjects { get; set; }
        public int TotalNodes { get; set; }
        public int TotalSubNodes { get; set; }
        public int CompletedProjects { get; set; }
        public double OverallProgress { get; set; }
        public List<NewIndexDepartmentProgressViewModel> DepartmentProgresses { get; set; } = new();
        public List<NewIndexRecentProjectViewModel> RecentProjects { get; set; } = new();
    }

    public class NewIndexRecentProjectViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; }
        public double CompletionProgress { get; set; }
        public DateTime? EndTime { get; set; }
        public int NodeCount { get; set; }
    }

    public class NewIndexDepartmentProgressViewModel
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalSubNodes { get; set; }
        public int CompletedSubNodes { get; set; }
        public double ProgressPercent { get; set; }
    }
}
