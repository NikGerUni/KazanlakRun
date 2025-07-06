
namespace KazanlakRun.Data.Models
{
    public class AidStationDistance
    {
        public int AidStationId { get; set; }
        public AidStation AidStation { get; set; } = null!;

        public int DistanceId { get; set; }
        public Distance Distance { get; set; } = null!;
    }
}

