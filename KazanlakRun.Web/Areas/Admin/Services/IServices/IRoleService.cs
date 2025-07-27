using KazanlakRun.Web.Areas.Admin.Models;

namespace KazanlakRun.Web.Areas.Admin.Services.IServices
{
    public interface IRoleService
    {
        Task<List<RoleViewModel>> GetAllAsync();
        Task SaveAllAsync(List<RoleViewModel> roles);
    }
}
