using KazanlakRun.GCommon;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KazanlakRun.Web.Areas.Admin.Models
{
    public class AidStationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(ValidationConstants.AidStationNameMaxLen,
             MinimumLength = ValidationConstants.AidStationNameMinLen,
             ErrorMessage = "Aid station name must be between {2} and {1} characters long.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Short name is required.")]
        [StringLength(
            ValidationConstants.AidStationShortNameMaxLen,
            MinimumLength = ValidationConstants.AidStationShortNameMinLen,
            ErrorMessage = "Short name must be between {2} and {1} characters long.")]
        public string ShortName { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> AllDistances { get; set; } = new List<SelectListItem>();
        public int[] SelectedDistanceIds { get; set; } = Array.Empty<int>();

        public IEnumerable<SelectListItem> AllVolunteers { get; set; } = new List<SelectListItem>();
        public int[] SelectedVolunteerIds { get; set; } = Array.Empty<int>();
    }
}
