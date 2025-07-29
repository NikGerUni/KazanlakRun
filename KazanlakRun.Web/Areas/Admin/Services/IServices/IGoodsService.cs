using KazanlakRun.Data.Models;

namespace KazanlakRun.Services.Contracts
{
    public interface IGoodsService
    {
        Task<List<Good>> GetAllAsync();
        Task<Good?> GetByIdAsync(int id);
        Task<Good> CreateAsync(Good good);
        Task<bool> UpdateAsync(Good good);
        Task<bool> DeleteAsync(int id);
        Task<List<Good>> SaveBatchAsync(List<Good> goods);
    }
}
