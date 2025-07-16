// Web/Areas/Admin/Models/AidStationViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KazanlakRun.Web.Areas.Admin.Models
{
    public class AidStationViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string ShortName { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> AllDistances { get; set; } = new List<SelectListItem>();
        public int[] SelectedDistanceIds { get; set; } = Array.Empty<int>();

        public IEnumerable<SelectListItem> AllVolunteers { get; set; } = new List<SelectListItem>();
        public int[] SelectedVolunteerIds { get; set; } = Array.Empty<int>();
    }
}