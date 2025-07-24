using AutoMapper;
using KazanlakRun.Areas.User.Models;
using KazanlakRun.Data;
using KazanlakRun.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace KazanlakRun.Web.Areas.User.Services
{
    public class VolunteerService : IVolunteerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public VolunteerService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateAsync(string userId, VolunteerInputModel model)
        {
            // мапваме InputModel -> Volunteer
            var volunteer = _mapper.Map<Volunteer>(model);

            volunteer.UserId = userId;
            volunteer.AidStationId = 1;           // временно
                                                  // други пропъртита...

            _context.Volunteers.Add(volunteer);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string userId)
            => await _context.Volunteers.AnyAsync(v => v.UserId == userId);

        public async Task<VolunteerInputModel?> GetByUserIdAsync(string userId)
        {
            var entity = await _context.Volunteers
                .FirstOrDefaultAsync(v => v.UserId == userId);

            if (entity == null) return null;

            // мапваме Volunteer -> InputModel
            return _mapper.Map<VolunteerInputModel>(entity);
        }

        public async Task UpdateAsync(string userId, VolunteerInputModel model)
        {
            var entity = await _context.Volunteers
                .FirstOrDefaultAsync(v => v.UserId == userId);

            if (entity == null)
                throw new InvalidOperationException("Registration not found.");

            // мапваме само променените полета от model върху entity
            _mapper.Map(model, entity);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string userId)
        {
            var entity = await _context.Volunteers
                .FirstOrDefaultAsync(v => v.UserId == userId);

            if (entity != null)
            {
                _context.Volunteers.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
