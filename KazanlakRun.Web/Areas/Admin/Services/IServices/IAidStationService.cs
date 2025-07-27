using KazanlakRun.Web.Areas.Admin.Models;

namespace KazanlakRun.Web.Areas.Admin.Services.IServices
{
    public interface IAidStationService
    {
        Task<List<AidStationListItem>> GetAllAsync();
        Task<AidStationViewModel> GetForCreateAsync();
        Task CreateAsync(AidStationViewModel model);
        Task<AidStationViewModel> GetForEditAsync(int id);
        Task UpdateAsync(AidStationViewModel model);
        Task DeleteAsync(int id);
    }
}
