using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using KazanlakRun.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [TypeFilter(typeof(ReportExceptionFilter))]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(
            IReportService reportService,
            ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        public async Task<IActionResult> RunnersByAidStation()
        {
            var model = await _reportService.GetRunnersByAidStationAsync();
            return View(model);
        }

        public async Task<IActionResult> VolunteersByAidStation(int page = 1, string filter = "")
        {
            var allStations = await _reportService.GetVolunteersByAidStationAsync();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                allStations = allStations
                    .Select(a => new AidStationVolunteersReportViewModel
                    {
                        AidStationName = a.AidStationName,
                        Volunteers = a.Volunteers
                            .Where(v =>
                                v.Names.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                                v.Email.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                                v.Phone.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                                v.Roles.Any(r => r.Contains(filter, StringComparison.OrdinalIgnoreCase))
                            )
                            .ToList()
                    })
                    .Where(a => a.Volunteers.Any())
                    .ToList();
            }

            const int pageSize = 1;
            var total = allStations.Count;
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);
            page = Math.Clamp(page, 1, Math.Max(1, totalPages));

            var station = allStations
                .Skip((page - 1) * pageSize)
                .FirstOrDefault()
                ?? new AidStationVolunteersReportViewModel
                {
                    AidStationName = "–",
                    Volunteers = new()
                };

            var vm = new VolunteersByAidStationPageViewModel
            {
                Station = station,
                PageNumber = page,
                TotalPages = totalPages,
                FilterText = filter
            };

            return View(vm);
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
