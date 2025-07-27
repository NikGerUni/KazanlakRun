using KazanlakRun.GCommon;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KazanlakRun.Web.Areas.Admin.Models
{
    public class AidStationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Името е задължително.")]
        [StringLength(ValidationConstants.AidStationNameMaxLen,
             MinimumLength = ValidationConstants.AidStationNameMinLen,
             ErrorMessage = "Името на помощна станция трябва да е между {2} и {1} символа.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Краткото име е задължително.")]
        [StringLength(
            ValidationConstants.AidStationShortNameMaxLen,
            MinimumLength = ValidationConstants.AidStationShortNameMinLen,
            ErrorMessage = "Краткото име трябва да е между {2} и {1} символа.")]
        public string ShortName { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> AllDistances { get; set; } = new List<SelectListItem>();
        public int[] SelectedDistanceIds { get; set; } = Array.Empty<int>();

        public IEnumerable<SelectListItem> AllVolunteers { get; set; } = new List<SelectListItem>();
        public int[] SelectedVolunteerIds { get; set; } = Array.Empty<int>();
    }
}
