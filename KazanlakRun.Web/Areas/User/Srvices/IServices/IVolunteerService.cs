using KazanlakRun.Areas.User.Models;

namespace KazanlakRun.Areas.User.Services
{
    public interface IVolunteerService
    {
        Task CreateAsync(string userId, VolunteerInputModel model);
    }
}

