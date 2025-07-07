
namespace KazanlakRun.Data.Models
{
    public class AidStation
    {
        public int Id { get; set; }
        public string ShortName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public ICollection<Volunteer> Volunteers { get; set; }  = new List<Volunteer>();
        public ICollection<AidStationDistance> AidStationDistances { get; set; }  = new List<AidStationDistance>();
       
    }
}

