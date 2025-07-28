
using KazanlakRun.Web.Services.IServices;
using Microsoft.Extensions.Caching.Memory;

namespace KazanlakRun.Web.Areas.Admin.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void ClearReportCache()
        {
            _cache.Remove("RunnersByAidStation");
            _cache.Remove("VolunteersByAidStation");
            _cache.Remove("GoodsByAidStation");
            _cache.Remove("GoodsForDelivery");
        }
    }
}
