// KazanlakRun.Services.Core/Services/AidStationService.cs
using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KazanlakRun.Web.Areas.Admin.Services
{
    public class AidStationService : IAidStationService
    {
        private readonly ApplicationDbContext _db;

        public AidStationService(ApplicationDbContext db) => _db = db;

        // KazanlakRun.Services.Core/Services/AidStationService.cs (метод GetAllAsync)
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


        //public async Task CreateAsync(AidStationViewModel model)
        //{
        //    var entity = new AidStation
        //    {
        //        Name = model.Name
        //    };

        //    foreach (var did in model.SelectedDistanceIds)
        //        entity.AidStationDistances
        //            .Add(new AidStationDistance { DistanceId = did });

        //    _db.AidStations.Add(entity);
        //    await _db.SaveChangesAsync();

        //    if (model.SelectedVolunteerIds.Any())
        //    {
        //        var vols = await _db.Volunteers
        //            .Where(v => model.SelectedVolunteerIds.Contains(v.Id))
        //            .ToListAsync();
        //        vols.ForEach(v => v.AidStationId = entity.Id);
        //        await _db.SaveChangesAsync();
        //    }
        //}

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
        }



        public async Task UpdateAsync(AidStationViewModel model)
        {
            var station = await _db.AidStations
                .Include(a => a.AidStationDistances)
                .Include(a => a.Volunteers)
                .FirstOrDefaultAsync(a => a.Id == model.Id)
                ?? throw new KeyNotFoundException();

            station.Name = model.Name;

            var toRemoveDist = station.AidStationDistances
                .Where(ad => !model.SelectedDistanceIds.Contains(ad.DistanceId))
                .ToList();
            _db.RemoveRange(toRemoveDist);

            var existingDist = station.AidStationDistances
                .Select(ad => ad.DistanceId)
                .ToList();
            var toAddDist = model.SelectedDistanceIds
                .Except(existingDist);
            foreach (var did in toAddDist)
                station.AidStationDistances
                    .Add(new AidStationDistance { DistanceId = did });

            var currentVolIds = station.Volunteers.Select(v => v.Id).ToList();
            var toRemoveVols = station.Volunteers
                .Where(v => !model.SelectedVolunteerIds.Contains(v.Id))
                .ToList();
            toRemoveVols.ForEach(v => v.AidStationId = 0);

            var toAddVols = await _db.Volunteers
                .Where(v => model.SelectedVolunteerIds.Except(currentVolIds).Contains(v.Id))
                .ToListAsync();
            toAddVols.ForEach(v => v.AidStationId = station.Id);

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var station = await _db.AidStations.FindAsync(id)
                ?? throw new KeyNotFoundException();
            _db.AidStations.Remove(station);
            await _db.SaveChangesAsync();
        }
    }
}

