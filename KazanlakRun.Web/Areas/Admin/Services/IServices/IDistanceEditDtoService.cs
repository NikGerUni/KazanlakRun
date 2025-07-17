using KazanlakRun.Web.Areas.Admin.Models;

namespace KazanlakRun.Web.Areas.Admin.Services.IServices
{
    public interface IDistanceEditDtoService
    {
        Task<IEnumerable<DistanceEditDto>> GetAllAsync();
        Task<DistanceEditDto?> GetByIdAsync(int id);
        Task UpdateAsync(DistanceEditDto distance);
        Task UpdateMultipleAsync(IEnumerable<DistanceEditDto> distances);
    }
}


