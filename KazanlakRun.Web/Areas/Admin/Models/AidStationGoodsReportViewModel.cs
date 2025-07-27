namespace KazanlakRun.Web.Areas.Admin.Models
{
    public class GoodReport
    {
        public string Name { get; set; } = null!;
        public string Measure { get; set; } = null!;
        public double QuantityPerAidStation { get; set; }
    }

    public class AidStationGoodsReportViewModel
    {
        public string AidStationName { get; set; } = null!;
        public int TotalRegisteredRunners { get; set; }
        public List<GoodReport> Goods { get; set; } = new();
    }
}
