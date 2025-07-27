// Distance.cs
using KazanlakRun.GCommon;
using System.ComponentModel.DataAnnotations;

namespace KazanlakRun.Data.Models
{
    public class Distance
    {
        public int Id { get; set; }
        public string Distans { get; set; } = null!;

        [Range(ValidationConstants.RegRunnersMinNumber, ValidationConstants.RegRunnersMaxNumber)]
        public int RegRunners { get; set; }

        public ICollection<AidStationDistance> AidStationDistances { get; set; }
            = new List<AidStationDistance>();
    }
}

