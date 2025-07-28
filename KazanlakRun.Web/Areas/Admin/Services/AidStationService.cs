using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using KazanlakRun.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace KazanlakRun.Web.Areas.Admin.Services
{
    public class AidStationService : IAidStationService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICacheService _cacheService;

        public AidStationService(ApplicationDbContext db, ICacheService cacheService)
        {
            _db = db;
            _cacheService = cacheService;
        }


        public async Task<List<AidStationListItem>> GetAllAsync() =>
            await _db.AidStations
                .Include(a => a.AidStationDistances).ThenInclude(ad => ad.Distance)
                .Include(a => a.Volunteers).ThenInclude(v => v.VolunteerRoles).ThenInclude(vr => vr.Role)
                .Select(a => new AidStationListItem
                {
                    Id = a.Id,
                    Name = a.Name,
                    DistanceNames = a.AidStationDistances
                                     .Select(ad => ad.Distance.Distans)
                                     .ToList(),
                    VolunteerDescriptions = a.Volunteers
                                     .Select(v => v.Names
                                         + (v.VolunteerRoles.Any()
                                             ? " – " + string.Join(", ",
                                                   v.VolunteerRoles
                                                    .Select(vr => vr.Role.Name))
                                             : ""))
                                     .ToList()
                })
                .ToListAsync();



        public async Task<AidStationViewModel> GetForCreateAsync()
        {
            var distances = await _db.Distances
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Distans
                })
                .ToListAsync();

            var volunteersRaw = await _db.Volunteers
                .Include(v => v.VolunteerRoles)
                    .ThenInclude(vr => vr.Role)
                .Select(v => new
                {
                    v.Id,
                    v.Names,
                    RoleNames = v.VolunteerRoles.Select(vr => vr.Role.Name)
                })
                .ToListAsync();

            var volunteers = volunteersRaw
                .Select(v => new SelectListItem
                {
                    Value = v.Id.ToString(),
                    Text = v.Names
                         + (v.RoleNames.Any()
                             ? " – " + string.Join(", ", v.RoleNames)
                             : "")
                })
                .ToList();

            return new AidStationViewModel
            {
                AllDistances = distances,
                AllVolunteers = volunteers
            };
        }


        public async Task<AidStationViewModel> GetForEditAsync(int id)
        {
            var station = await _db.AidStations
                .Include(a => a.AidStationDistances)
                .Include(a => a.Volunteers)
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new KeyNotFoundException();

            var selectedDistanceIds = station.AidStationDistances
                .Select(ad => ad.DistanceId)
                .ToHashSet();
            var selectedVolunteerIds = station.Volunteers
                .Select(v => v.Id)
                .ToHashSet();

            var distances = await _db.Distances
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Distans,
                    Selected = selectedDistanceIds.Contains(d.Id)
                })
                .ToListAsync();

            var volunteersRaw = await _db.Volunteers
                .Include(v => v.VolunteerRoles)
                    .ThenInclude(vr => vr.Role)
                .Select(v => new
                {
                    v.Id,
                    v.Names,
                    RoleNames = v.VolunteerRoles.Select(vr => vr.Role.Name)
                })
                .ToListAsync();

            var volunteers = volunteersRaw
                .Select(v => new SelectListItem
                {
                    Value = v.Id.ToString(),
                    Text = v.Names
                         + (v.RoleNames.Any()
                             ? " – " + string.Join(", ", v.RoleNames)
                             : ""),
                    Selected = selectedVolunteerIds.Contains(v.Id)
                })
                .ToList();

            return new AidStationViewModel
            {
                Id = station.Id,
                ShortName = station.ShortName,
                Name = station.Name,
                AllDistances = distances,
                SelectedDistanceIds = selectedDistanceIds.ToArray(),
                AllVolunteers = volunteers,
                SelectedVolunteerIds = selectedVolunteerIds.ToArray()
            };
        }




        public async Task CreateAsync(AidStationViewModel model)
        {
            var entity = new AidStation { ShortName = model.ShortName, Name = model.Name };

            var distanceIds = model.SelectedDistanceIds ?? Array.Empty<int>();
            foreach (var did in distanceIds)
                entity.AidStationDistances.Add(new AidStationDistance { DistanceId = did });

            _db.AidStations.Add(entity);
            await _db.SaveChangesAsync();

            var volunteerIds = model.SelectedVolunteerIds ?? Array.Empty<int>();
            if (volunteerIds.Length > 0)
            {
                var vols = await _db.Volunteers
                    .Where(v => volunteerIds.Contains(v.Id))
                    .ToListAsync();

                vols.ForEach(v => v.AidStationId = entity.Id);
                await _db.SaveChangesAsync();
            }
            _cacheService.ClearReportCache();
        }



        public async Task UpdateAsync(AidStationViewModel model)
        {
            var station = await _db.AidStations
                .Include(a => a.AidStationDistances)
                .Include(a => a.Volunteers)
                .FirstOrDefaultAsync(a => a.Id == model.Id)
                ?? throw new KeyNotFoundException();


            station.Name = model.Name;
            station.ShortName = model.ShortName;



            var selectedDistanceIds = model.SelectedDistanceIds ?? Array.Empty<int>();


            var toRemoveDist = station.AidStationDistances
                .Where(ad => !selectedDistanceIds.Contains(ad.DistanceId))
                .ToList();
            _db.AidStationDistances.RemoveRange(toRemoveDist);


            var existingDistanceIds = station.AidStationDistances.Select(ad => ad.DistanceId).ToHashSet();
            var toAddDist = selectedDistanceIds.Except(existingDistanceIds);
            foreach (var did in toAddDist)
            {
                station.AidStationDistances.Add(new AidStationDistance { DistanceId = did });
            }



            var selectedVolunteerIds = model.SelectedVolunteerIds ?? Array.Empty<int>();


            foreach (var vol in station.Volunteers.ToList())
            {
                vol.AidStationId = null;
            }

            if (selectedVolunteerIds.Any())
            {
                var newVolunteers = await _db.Volunteers
                    .Where(v => selectedVolunteerIds.Contains(v.Id))
                    .ToListAsync();

                foreach (var vol in newVolunteers)
                {
                    vol.AidStationId = station.Id;
                }
            }
            await _db.SaveChangesAsync();

            _cacheService.ClearReportCache();
        }


        public async Task DeleteAsync(int id)
        {
            var station = await _db.AidStations.FindAsync(id)
                ?? throw new KeyNotFoundException();
            _db.AidStations.Remove(station);
            await _db.SaveChangesAsync();

            _cacheService.ClearReportCache();
        }
    }
}

