// Distance.cs
namespace KazanlakRun.Data.Models
{
    public class Distance
    {
        public int Id { get; set; }

        /// <summary>Име на дистанцията (max 10)</summary>
        public string Distans { get; set; } = null!;

        public int RegRunners { get; set; }

        public ICollection<AidStationDistance> AidStationDistances { get; set; }
            = new List<AidStationDistance>();
    }
}

