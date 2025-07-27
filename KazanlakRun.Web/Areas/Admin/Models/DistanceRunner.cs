namespace KazanlakRun.Web.Areas.Admin.Models
{
    public class DistanceRunner
    {
        public string DistanceName { get; set; } = null!;
        public int RegRunners { get; set; }
    }

    public class AidStationRunnersReportViewModel
    {
        public string AidStationName { get; set; } = null!;
        public List<DistanceRunner> Distances { get; set; } = new();

        public int TotalRunners
            => Distances.Sum(d => d.RegRunners);
    }
}
