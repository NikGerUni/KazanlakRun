// AidStationDistance.cs
namespace KazanlakRun.Data.Models
{
    /// <summary>Join entity за many-to-many AidStation ↔ Distance</summary>
    public class AidStationDistance
    {
        public int AidStationId { get; set; }
        public AidStation AidStation { get; set; } = null!;

        public int DistanceId { get; set; }
        public Distance Distance { get; set; } = null!;
    }
}

