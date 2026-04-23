using Microsoft.AspNetCore.Identity;

namespace SkylinePlanManagementSystem.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string City { get; set; }
    }
}
