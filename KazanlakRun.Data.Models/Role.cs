
using KazanlakRun.GCommon;
using System.ComponentModel.DataAnnotations;

namespace KazanlakRun.Data.Models
{
    public class Role
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Името на ролята е задължително.")]
        [StringLength(
        ValidationConstants.RoleMaxLen,
        MinimumLength = ValidationConstants.RoleMinLen,
        ErrorMessage = "Името на ролята трябва да е между {2} и {1} символа.")]
        public string Name { get; set; } = null!;

        public ICollection<VolunteerRole> VolunteerRoles { get; set; }
            = new List<VolunteerRole>();
    }
}

