// KazanlakRun.Web.Areas.Admin.Models/AidStationViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KazanlakRun.Web.Areas.Admin.Models
{
    public class AidStationViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [Display(Name = "Distances")]
        public List<SelectListItem> AllDistances { get; set; }
            = new List<SelectListItem>();

        [Display(Name = "Selected Distances")]
        public int[] SelectedDistanceIds { get; set; }
            = Array.Empty<int>();

        [Display(Name = "Volunteers")]
        public List<SelectListItem> AllVolunteers { get; set; }
            = new List<SelectListItem>();

        [Display(Name = "Selected Volunteers")]
        public int[] SelectedVolunteerIds { get; set; }
            = Array.Empty<int>();
    }
}

