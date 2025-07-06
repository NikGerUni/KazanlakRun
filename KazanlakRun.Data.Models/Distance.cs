// Distance.cs
namespace KazanlakRun.Data.Models
{
    public class Distance
    {
        public int Id { get; set; }

   
        public string Distans { get; set; } = null!;

        public int RegRunners { get; set; }

        public ICollection<AidStationDistance> AidStationDistances { get; set; }
            = new List<AidStationDistance>();
    }
}

