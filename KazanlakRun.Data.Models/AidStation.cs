using KazanlakRun.GCommon;
using System.ComponentModel.DataAnnotations;

namespace KazanlakRun.Data.Models
{
    public class AidStation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Short name is required.")]
        [StringLength(
            ValidationConstants.AidStationShortNameMaxLen,
            MinimumLength = ValidationConstants.AidStationShortNameMinLen,
            ErrorMessage = "Short name must be between {2} and {1} characters long.")]
        public string ShortName { get; set; } = null!;

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(
            ValidationConstants.AidStationNameMaxLen,
            MinimumLength = ValidationConstants.AidStationNameMinLen,
            ErrorMessage = "Aid station name must be between {2} and {1} characters long.")]
        public string Name { get; set; } = null!;

        public ICollection<Volunteer> Volunteers { get; set; } = new List<Volunteer>();
        public ICollection<AidStationDistance> AidStationDistances { get; set; } = new List<AidStationDistance>();
    }
}
