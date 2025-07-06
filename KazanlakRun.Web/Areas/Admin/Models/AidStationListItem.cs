// KazanlakRun.Web.Areas.Admin.Models/AidStationListItem.cs
namespace KazanlakRun.Web.Areas.Admin.Models
{
    public class AidStationListItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<string> DistanceNames { get; set; } = new();
        public List<string> VolunteerDescriptions { get; set; } = new();
    }
}
