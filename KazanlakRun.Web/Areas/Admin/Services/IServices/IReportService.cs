using System.Collections.Generic;
using System.Threading.Tasks;
using KazanlakRun.Web.Areas.Admin.Models;

namespace KazanlakRun.Web.Areas.Admin.Services.IServices
{
    public interface IReportService
    {
        Task<List<AidStationRunnersReportViewModel>> GetRunnersByAidStationAsync();
        Task<List<AidStationVolunteersReportViewModel>> GetVolunteersByAidStationAsync();
        Task<List<AidStationGoodsReportViewModel>> GetGoodsByAidStationAsync();
        Task<List<GoodsForDeliveryReportViewModel>> GetGoodsForDeliveryAsync();
    }
}
