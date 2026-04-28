using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SkylinePlanManagementSystem.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; } = string.Empty;

        public int? DepartmentId { get; set; }

        public Department? Department { get; set; }

        [Display(Name = "账户有效")] 
        public bool IsActive { get; set; } = true;
    }
}
