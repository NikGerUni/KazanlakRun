using KazanlakRun.Data.Models;


namespace  KazanlakRun.Web.Areas.Admin.Services.IServices
{
    public interface IDistanceService
    {
        Task<IEnumerable<Distance>> GetAllAsync();
        Task<Distance?> GetByIdAsync(int id);
        Task UpdateAsync(Distance distance);
        Task UpdateMultipleAsync(IEnumerable<Distance> distances);
    }
}

