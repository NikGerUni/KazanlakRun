using AutoMapper;
using KazanlakRun.Data;
using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.User.Models;

namespace KazanlakRun.Web.Areas.User.Services
{
    public class VolunteerService : IVolunteerService
    {
        private readonly IRepository<Volunteer> _volunteerRepository;
        private readonly IMapper _mapper;

        public VolunteerService(IRepository<Volunteer> volunteerRepository, IMapper mapper)
        {
            _volunteerRepository = volunteerRepository;
            _mapper = mapper;
        }

        public async Task CreateAsync(string userId, VolunteerInputModel model)
        {
            var volunteer = _mapper.Map<Volunteer>(model);
            volunteer.UserId = userId;
            volunteer.AidStationId = 1;

            await _volunteerRepository.AddAsync(volunteer);
            await _volunteerRepository.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string userId)
        {
            return await _volunteerRepository.AnyAsync(v => v.UserId == userId);
        }

        public async Task<VolunteerInputModel?> GetByUserIdAsync(string userId)
        {
            var volunteers = await _volunteerRepository.FindAsync(v => v.UserId == userId);
            var entity = volunteers.FirstOrDefault();

            if (entity == null) return null;

            return _mapper.Map<VolunteerInputModel>(entity);
        }

        public async Task DeleteAsync(string userId)
        {
            var volunteers = await _volunteerRepository.FindAsync(v => v.UserId == userId);
            var entity = volunteers.FirstOrDefault();

            if (entity != null)
            {
                _volunteerRepository.Remove(entity);
                await _volunteerRepository.SaveChangesAsync(); // ✅ добавено
            }
        }


        public async Task UpdateAsync(string userId, VolunteerInputModel model)
        {
            var volunteers = await _volunteerRepository.FindAsync(v => v.UserId == userId);
            var entity = volunteers.FirstOrDefault();

            if (entity == null)
                throw new InvalidOperationException("Registration not found.");

            _mapper.Map(model, entity);
            _volunteerRepository.Update(entity);

            await _volunteerRepository.SaveChangesAsync(); // ✅ добавено
        }

    }
}