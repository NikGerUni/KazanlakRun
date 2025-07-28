using Microsoft.AspNetCore.Mvc;

namespace KazanlakRun.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : Controller
    {


        [Route("Error/500")]
        public IActionResult ExceptionHandler()
        {
            return View("500");
        }
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            Response.StatusCode = statusCode;
            ViewData["ErrorCode"] = statusCode;
            return View(statusCode.ToString());
        }
    }
}
