// KazanlakRun.Services.Core/Services/AidStationService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KazanlakRun.Web.Areas.Admin.Services;
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

        public async Task<List<AidStationListItem>> GetAllAsync() =>
            await _db.AidStations
                .Include(a => a.AidStationDistances)
                    .ThenInclude(ad => ad.Distance)
                .Include(a => a.Volunteers)
                .Select(a => new AidStationListItem
                {
                    Id = a.Id,
                    Name = a.Name,
                    DistanceNames = a.AidStationDistances
                        .Select(ad => ad.Distance.Distans)
                        .ToList(),
                    VolunteerNames = a.Volunteers
                        .Select(v => v.Names)
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

            var volunteers = await _db.Volunteers
                .Select(v => new SelectListItem
                {
                    Value = v.Id.ToString(),
                    Text = v.Names
                })
                .ToListAsync();

            return new AidStationViewModel
            {
                AllDistances = distances,
                AllVolunteers = volunteers
            };
        }

        public async Task CreateAsync(AidStationViewModel model)
        {
            var entity = new AidStation
            {
                Name = model.Name
            };

            foreach (var did in model.SelectedDistanceIds)
                entity.AidStationDistances
                    .Add(new AidStationDistance { DistanceId = did });

            _db.AidStations.Add(entity);
            await _db.SaveChangesAsync();

            if (model.SelectedVolunteerIds.Any())
            {
                var vols = await _db.Volunteers
                    .Where(v => model.SelectedVolunteerIds.Contains(v.Id))
                    .ToListAsync();
                vols.ForEach(v => v.AidStationId = entity.Id);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<AidStationViewModel> GetForEditAsync(int id)
        {
            var station = await _db.AidStations
                .Include(a => a.AidStationDistances)
                .Include(a => a.Volunteers)
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new KeyNotFoundException();

            // Извличаме избраните Id-та в паметта
            var selectedDistanceIds = station.AidStationDistances
                .Select(ad => ad.DistanceId)
                .ToHashSet();
            var selectedVolunteerIds = station.Volunteers
                .Select(v => v.Id)
                .ToHashSet();

            // Взимаме всички разстояния и след това мапваме Selected локално
            var distances = await _db.Distances
                .Select(d => new { d.Id, d.Distans })
                .ToListAsync();

            var allDistances = distances
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Distans,
                    Selected = selectedDistanceIds.Contains(d.Id)
                })
                .ToList();

            // Същото и за доброволците
            var volunteersRaw = await _db.Volunteers
                .Select(v => new { v.Id, v.Names })
                .ToListAsync();

            var allVolunteers = volunteersRaw
                .Select(v => new SelectListItem
                {
                    Value = v.Id.ToString(),
                    Text = v.Names,
                    Selected = selectedVolunteerIds.Contains(v.Id)
                })
                .ToList();

            return new AidStationViewModel
            {
                Id = station.Id,
                Name = station.Name,
                AllDistances = allDistances,
                SelectedDistanceIds = selectedDistanceIds.ToArray(),
                AllVolunteers = allVolunteers,
                SelectedVolunteerIds = selectedVolunteerIds.ToArray()
            };
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

