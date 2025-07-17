// Distance.cs
using System.ComponentModel.DataAnnotations;
using KazanlakRun.GCommon;

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

