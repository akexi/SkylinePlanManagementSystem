using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MockSchoolManagement.Controllers
{
    public class ErrorController : Controller
    {
        private ILogger<ErrorController> logger;

        /// <summary>
        /// 注入ILogger服务
        /// 将控制器类型指定为泛型参数
        /// 有助于确定哪个类生成了日志，然后记录它
        /// </summary>
        /// <param name="logger"></param>
        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        //[Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            // 获取异常细节
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            // 将异常记录作为日志中的错误类别记录
            logger.LogError($"路径 {exceptionHandlerPathFeature.Path} " + $"产生了一个错误{exceptionHandlerPathFeature.Error}");
            return View("Error");
        }


        public IActionResult Index()
        {
            return View();
        }

        // 如果状态码是404，则路径为 /Error/404
        [Route("Error/{statusCode}")]
        [HttpGet]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            // 获取异常细节
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "抱歉，您访问的页面不存在";
                    // 将异常记录作为日志中的警告类别记录
                    logger.LogWarning($"发生了一个404错误. 路径 = " +
                        $"{statusCodeResult.OriginalPath} 以及查询字符串 = " +
                        $"{statusCodeResult.OriginalQueryString}");
                    break;
            }

            return View("NotFound");
        }
    }
}
