using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KazanlakRun.Web.Areas.Admin.Models
{
    public class VolunteerViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Име")]
        public string Names { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "Имейл")]
        public string Email { get; set; } = null!;

        [Required]
        [Phone]
        [Display(Name = "Телефон")]
        public string Phone { get; set; } = null!;

        // Това ще се ползва за чекбоксовете / <select multiple>
        [Display(Name = "Роли")]
        public List<SelectListItem> AllRoles { get; set; }
            = new List<SelectListItem>();

        // Тук ще пристигнат избраните роли от формата
        public int[] SelectedRoleIds { get; set; } = Array.Empty<int>();
    }
}
