using Microsoft.AspNetCore.Identity;

namespace SkylinePlanManagementSystem.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; } = string.Empty;

        public int? DepartmentId { get; set; }

        public Department? Department { get; set; }
    }
}
