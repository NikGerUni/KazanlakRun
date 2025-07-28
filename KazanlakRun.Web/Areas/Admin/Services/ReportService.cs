using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace KazanlakRun.Web.Areas.Admin.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ReportService> _logger;
        private readonly IMemoryCache _cache;

        public ReportService(ApplicationDbContext db, ILogger<ReportService> logger, IMemoryCache cache)
        {
            _db = db;
            _logger = logger;
            _cache = cache;
        }

        public async Task<List<AidStationRunnersReportViewModel>> GetRunnersByAidStationAsync()
        {
            const string cacheKey = "RunnersByAidStation";
            if (_cache.TryGetValue(cacheKey, out List<AidStationRunnersReportViewModel> cached))
                return cached;

            var stations = await _db.AidStations
                .Include(a => a.AidStationDistances).ThenInclude(ad => ad.Distance)
                .AsNoTracking()
                .ToListAsync();

            var result = stations
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

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        public async Task<List<AidStationVolunteersReportViewModel>> GetVolunteersByAidStationAsync()
        {
            const string cacheKey = "VolunteersByAidStation";
            if (_cache.TryGetValue(cacheKey, out List<AidStationVolunteersReportViewModel> cached))
                return cached;

            var stations = await _db.AidStations
                .Include(a => a.Volunteers)
                    .ThenInclude(v => v.VolunteerRoles)
                        .ThenInclude(vr => vr.Role)
                .AsNoTracking()
                .ToListAsync();

            var result = stations
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

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        public async Task<List<AidStationGoodsReportViewModel>> GetGoodsByAidStationAsync()
        {
            const string cacheKey = "GoodsByAidStation";
            if (_cache.TryGetValue(cacheKey, out List<AidStationGoodsReportViewModel> cached))
                return cached;

            var goods = await _db.Goods
                .Where(g => g.QuantityOneRunner > 0)
                .AsNoTracking()
                .ToListAsync();

            var stations = await _db.AidStations
                .Include(a => a.AidStationDistances)
                    .ThenInclude(ad => ad.Distance)
                .AsNoTracking()
                .ToListAsync();

            var result = stations
                .Select(a =>
                {
                    var totalRunners = a.AidStationDistances.Sum(ad => ad.Distance.RegRunners);
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

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        public async Task<List<GoodsForDeliveryReportViewModel>> GetGoodsForDeliveryAsync()
        {
            const string cacheKey = "GoodsForDelivery";
            if (_cache.TryGetValue(cacheKey, out List<GoodsForDeliveryReportViewModel> cached))
                return cached;

            var stations = await _db.AidStations
                .Include(a => a.AidStationDistances)
                    .ThenInclude(ad => ad.Distance)
                .AsNoTracking()
                .ToListAsync();

            var runnersPerStation = stations
                .Select(a => a.AidStationDistances.Sum(ad => ad.Distance.RegRunners))
                .ToList();

            var goods = await _db.Goods
                .AsNoTracking()
                .ToListAsync();

            var result = goods
                .Select(g =>
                {
                    var needed = runnersPerStation.Sum(r => r * g.QuantityOneRunner);
                    return new GoodsForDeliveryReportViewModel
                    {
                        Name = g.Name,
                        Measure = g.Measure ?? string.Empty,
                        NeededQuantity = (decimal)needed,
                        Quantity = (decimal)(g.Quantity ?? 0)
                    };
                })
                .ToList();

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }
    }
}
