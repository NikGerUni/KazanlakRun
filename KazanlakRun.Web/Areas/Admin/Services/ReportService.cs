using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KazanlakRun.Data;
using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;

namespace KazanlakRun.Web.Areas.Admin.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _db;

        public ReportService(ApplicationDbContext db)
            => _db = db;

        public async Task<List<AidStationRunnersReportViewModel>> GetRunnersByAidStationAsync()
        {
            var stations = await _db.AidStations
                .Include(a => a.AidStationDistances)
                    .ThenInclude(ad => ad.Distance)
                .ToListAsync();

            return stations
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
        }

        public async Task<List<AidStationVolunteersReportViewModel>> GetVolunteersByAidStationAsync()
        {
            var stations = await _db.AidStations
                .Include(a => a.Volunteers)
                    .ThenInclude(v => v.VolunteerRoles)
                        .ThenInclude(vr => vr.Role)
                .ToListAsync();

            return stations
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
        }

        public async Task<List<AidStationGoodsReportViewModel>> GetGoodsByAidStationAsync()
        {
            var goods = await _db.Goods
                .Where(g => g.QuantityOneRunner > 0)
                .ToListAsync();

            var stations = await _db.AidStations
                .Include(a => a.AidStationDistances)
                    .ThenInclude(ad => ad.Distance)
                .ToListAsync();

            return stations
                .Select(a =>
                {
                    var totalRunners = a.AidStationDistances
                        .Sum(ad => ad.Distance.RegRunners);

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
        }

        public async Task<List<GoodsForDeliveryReportViewModel>> GetGoodsForDeliveryAsync()
        {
            var stations = await _db.AidStations
                .Include(a => a.AidStationDistances)
                    .ThenInclude(ad => ad.Distance)
                .ToListAsync();

            var runnersPerStation = stations
                .Select(a => a.AidStationDistances.Sum(ad => ad.Distance.RegRunners))
                .ToList();

            var goods = await _db.Goods.ToListAsync();

            return goods
                .Select(g =>
                {
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
        }
    }
}
