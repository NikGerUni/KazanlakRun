using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KazanlakRun.Data;
using KazanlakRun.Web.Areas.Admin.Models;

namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ReportController(ApplicationDbContext db)
            => _db = db;

        public async Task<IActionResult> RunnersByAidStation()
        {
            var stations = await _db.AidStations
                .Include(a => a.AidStationDistances)
                    .ThenInclude(ad => ad.Distance)
                .ToListAsync();

            var model = stations
                .Select(a => new AidStationRunnersReportViewModel
                {
                    AidStationName = a.Name,
                    Distances = a.AidStationDistances
                        .Select(ad => new DistanceRunner
                        {
                            DistanceName = ad.Distance.Distans,
                            RegRunners = ad.Distance.RegRunners
                        })
                        .ToList()
                })
                .ToList();

            return View(model);
        }
        public async Task<IActionResult> VolunteersByAidStation()
        {
            var stations = await _db.AidStations
                .Include(a => a.Volunteers)
                    .ThenInclude(v => v.VolunteerRoles)
                        .ThenInclude(vr => vr.Role)
                .ToListAsync();

            var model = stations
                .Select(a => new AidStationVolunteersReportViewModel
                {
                    AidStationName = a.Name,
                    Volunteers = a.Volunteers
                        .Select(v => new VolunteerReport
                        {
                            Names = v.Names,
                            Email = v.Email,
                            Phone = v.Phone,
                            Roles = v.VolunteerRoles
                                        .Select(vr => vr.Role.Name)
                                        .ToList()
                        })
                        .ToList()
                })
                .ToList();

            return View(model);
        }
        public async Task<IActionResult> GoodsByAidStation()
        {
            // Зареждаме само тези Goods с Quantity > 0
            var goods = await _db.Goods
                                 .Where(g => g.QuantityOneRunner  > 0)
                                 .ToListAsync();

            // Зареждаме всички AidStations с техните дистанции
            var stations = await _db.AidStations
                .Include(a => a.AidStationDistances)
                    .ThenInclude(ad => ad.Distance)
                .ToListAsync();

            var model = stations
                .Select(a =>
                {
                    // общ брой регистрирани бегачи за тази станция
                    var totalRunners = a.AidStationDistances
                                        .Sum(ad => ad.Distance.RegRunners);

                    // изчисляваме потреблението за всяка стока
                    var goodsReport = goods
                        .Select(g => new GoodReport
                        {
                            Name = g.Name,
                            Measure = g.Measure,
                            QuantityPerAidStation = totalRunners * g.QuantityOneRunner
                        })
                        .ToList();

                    return new AidStationGoodsReportViewModel
                    {
                        AidStationName = a.Name,
                        TotalRegisteredRunners = totalRunners,
                        Goods = goodsReport
                    };
                })
                .ToList();

            return View(model);
        }
        // GET: Admin/Report/GoodsForDelivery
        public async Task<IActionResult> GoodsForDelivery()
        {
            // Взимаме всичките станции заедно с техните регистрирани бегачи
            var stations = await _db.AidStations
                .Include(a => a.AidStationDistances)
                    .ThenInclude(ad => ad.Distance)
                .ToListAsync();

            // За всяка станция – общ брой регистрирани бегачи
            var runnersPerStation = stations
                .Select(a => a.AidStationDistances.Sum(ad => ad.Distance.RegRunners))
                .ToList();

            // Зареждаме всички стоки
            var goods = await _db.Goods.ToListAsync();

            // За всяка стока изчисляваме:
            //   NeededQuantity = Σ(регистрирани бегачи на станция * количество на бегач)
            //   Quantity       = наличното количество в склада (Good.Quantity)
            var model = goods.Select(g =>
            {
                // Σ(регистрирани бегачи на станция * g.QuantityOneRunner)
                var needed = runnersPerStation
                    .Sum(runners => runners * g.QuantityOneRunner);

                return new GoodsForDeliveryReportViewModel
                {
                    Name = g.Name,
                    Measure = g.Measure ?? string.Empty,
                    NeededQuantity = (decimal)needed,
                    Quantity = (decimal)(g.Quantity ?? 0)
                };
            })
            .ToList();

            return View(model);
        }

    }
}
