using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SkylinePlanManagementSystem.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class SomeController : Controller
    {
        public string ABC()
        {
            return "我是方法ABC，只有拥有Admin或User角色即可访问我。";
        }

        [Authorize(Roles = "Admin")]
        public string XYZ() 
        {
            return "我是方法XYZ，只有Admin才访问我";
        }


        [AllowAnonymous]
        public string Anyone()
        {
            return "任何人都可以访问Anyone()，因为我添加了AllowAnonymous属性。";
        }
    }
}
