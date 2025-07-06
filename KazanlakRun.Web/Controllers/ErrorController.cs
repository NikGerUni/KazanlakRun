using Microsoft.AspNetCore.Mvc;

namespace KazanlakRun.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : Controller
    {


        // за необработени изключения, 500 Internal Server Error
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
            return View(statusCode.ToString());  // търси Views/Error/404.cshtml, 500.cshtml и т.н.
        }
    }
}
