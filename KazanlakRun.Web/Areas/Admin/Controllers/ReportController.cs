using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using KazanlakRun.Web.Areas.Admin.Models;

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

        public async Task<IActionResult> VolunteersByAidStation(int page = 1, string filter = "")
        {
            var allStations = await _reportService.GetVolunteersByAidStationAsync();

            // Server-side filtering
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
                            ).ToList()
                    })
                    .Where(a => a.Volunteers.Any())
                    .ToList();
            }

            int total = allStations.Count;
            int pageSize = 1; // по 1 станция на страница
            int totalPages = (int)Math.Ceiling(total / (double)pageSize);

            // Граници на page
            page = Math.Clamp(page, 1, Math.Max(1, totalPages));

            var station = allStations
                .Skip((page - 1) * pageSize)
                .FirstOrDefault()
                ?? new AidStationVolunteersReportViewModel { AidStationName = "–", Volunteers = new() };

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
