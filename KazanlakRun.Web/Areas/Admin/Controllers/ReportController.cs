using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using KazanlakRun.Web.Areas.Admin.Models;

namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportService reportService, ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        public async Task<IActionResult> RunnersByAidStation()
        {
            try
            {
                var model = await _reportService.GetRunnersByAidStationAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading runners by aid station report");
                TempData["Error"] = "Неуспешно зареждане на справката за бегачите. Моля, опитайте отново по-късно.";
                return View(Enumerable.Empty<AidStationRunnersReportViewModel>());
            }
        }

        public async Task<IActionResult> VolunteersByAidStation(int page = 1, string filter = "")
        {
            try
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

                int pageSize = 1;
                int total = allStations.Count;
                int totalPages = (int)Math.Ceiling(total / (double)pageSize);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading volunteers by aid station report");
                TempData["Error"] = "Неуспешно зареждане на справката за доброволците. Моля, опитайте отново по-късно.";
                var emptyVm = new VolunteersByAidStationPageViewModel
                {
                    Station = new AidStationVolunteersReportViewModel { AidStationName = "–", Volunteers = new() },
                    PageNumber = 1,
                    TotalPages = 1,
                    FilterText = filter
                };
                return View(emptyVm);
            }
        }

        public async Task<IActionResult> GoodsByAidStation()
        {
            try
            {
                var model = await _reportService.GetGoodsByAidStationAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading goods consumption report");
                TempData["Error"] = "Неуспешно зареждане на справката за консумация на стоки. Моля, опитайте отново по-късно.";
                return View(Enumerable.Empty<AidStationGoodsReportViewModel>());
            }
        }

        public async Task<IActionResult> GoodsForDelivery()
        {
            try
            {
                var model = await _reportService.GetGoodsForDeliveryAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading goods for delivery report");
                TempData["Error"] = "Неуспешно зареждане на справката за доставка на стоки. Моля, опитайте отново по-късно.";
                return View(Enumerable.Empty<GoodsForDeliveryReportViewModel>());
            }
        }
    }
}