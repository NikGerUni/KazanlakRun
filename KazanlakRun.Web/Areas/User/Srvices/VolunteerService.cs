using KazanlakRun.Areas.User.Models;
using KazanlakRun.Data;
using KazanlakRun.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace KazanlakRun.Areas.User.Services
{
    public class VolunteerService : IVolunteerService
    {
        private readonly ApplicationDbContext _context;

        public VolunteerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(string userId, VolunteerInputModel model)
        {
            var volunteer = new Volunteer
            {
                Names = model.Names,
                Email = model.Email,
                Phone = model.Phone,
                UserId = userId,
                AidStationId = 1 // temporary
            };
            _context.Volunteers.Add(volunteer);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string userId)
        {
            return await _context.Volunteers
                                 .AnyAsync(v => v.UserId == userId);
        }

        public async Task<VolunteerInputModel?> GetByUserIdAsync(string userId)
        {
            var v = await _context.Volunteers
                                  .FirstOrDefaultAsync(x => x.UserId == userId);
            if (v == null) return null;
            return new VolunteerInputModel
            {
                Names = v.Names,
                Email = v.Email,
                Phone = v.Phone
            };
        }

        public async Task UpdateAsync(string userId, VolunteerInputModel model)
        {
            var v = await _context.Volunteers
                                  .FirstOrDefaultAsync(x => x.UserId == userId);
            if (v == null) throw new InvalidOperationException("Registration not found.");
            v.Names = model.Names;
            v.Email = model.Email;
            v.Phone = model.Phone;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string userId)
        {
            var v = await _context.Volunteers
                                  .FirstOrDefaultAsync(x => x.UserId == userId);
            if (v != null)
            {
                _context.Volunteers.Remove(v);
                await _context.SaveChangesAsync();
            }
        }
    }

}
