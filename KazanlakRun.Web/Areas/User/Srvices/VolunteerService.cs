using KazanlakRun.Areas.User.Models;
using KazanlakRun.Data;
using KazanlakRun.Data.Models;

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
                AidStationId = 1 // временно — по-късно ще добавиш избор
            };

            _context.Volunteers.Add(volunteer);
            await _context.SaveChangesAsync();
        }
    }
}
