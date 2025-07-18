using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KazanlakRun.Web.Areas.Admin.Services.IServices;

namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
            => _reportService = reportService;

        public async Task<IActionResult> RunnersByAidStation()
        {
            var model = await _reportService.GetRunnersByAidStationAsync();
            return View(model);
        }

        public async Task<IActionResult> VolunteersByAidStation()
        {
            var model = await _reportService.GetVolunteersByAidStationAsync();
            return View(model);
        }

        public async Task<IActionResult> GoodsByAidStation()
        {
            var model = await _reportService.GetGoodsByAidStationAsync();
            return View(model);
        }

        public async Task<IActionResult> GoodsForDelivery()
        {
            var model = await _reportService.GetGoodsForDeliveryAsync();
            return View(model);
        }
    }
}
