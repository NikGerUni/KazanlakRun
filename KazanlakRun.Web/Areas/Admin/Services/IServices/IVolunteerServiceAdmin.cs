


using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Models;

namespace KazanlakRun.Web.Areas.Admin.Services.IServices
{
    public interface IVolunteerServiceAdmin
    {
        Task<List<Role>> GetAllRolesAsync();
        Task<List<VolunteerListItem>> GetAllVolunteersAsync();
        Task<VolunteerViewModel> GetForCreateAsync();
        Task CreateAsync(VolunteerViewModel model);
        Task<VolunteerViewModel> GetForEditAsync(int id);
        Task UpdateAsync(VolunteerViewModel model);
        Task DeleteAsync(int id);
    }
}
