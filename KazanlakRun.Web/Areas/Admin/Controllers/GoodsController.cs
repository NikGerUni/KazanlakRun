using Microsoft.AspNetCore.Mvc;

namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class GoodsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

