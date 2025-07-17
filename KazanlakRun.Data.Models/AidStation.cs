
// Data/Models/AidStation.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KazanlakRun.GCommon;

namespace KazanlakRun.Data.Models
{
    public class AidStation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Краткото име е задължително.")]
        [StringLength(
            ValidationConstants.AidStationShortNameMaxLen,
            MinimumLength = ValidationConstants.AidStationShortNameMinLen,
            ErrorMessage = "Краткото име трябва да е между {2} и {1} символа.")]
        public string ShortName { get; set; } = null!;

        [Required(ErrorMessage = "Името е задължително.")]
        [StringLength(
            ValidationConstants.AidStationNameMaxLen,
            MinimumLength = ValidationConstants.AidStationNameMinLen,
            ErrorMessage = "Името на помощната станция трябва да е между {2} и {1} символа.")]
        public string Name { get; set; } = null!;

        public ICollection<Volunteer> Volunteers { get; set; } = new List<Volunteer>();
        public ICollection<AidStationDistance> AidStationDistances { get; set; } = new List<AidStationDistance>();
    }
}


