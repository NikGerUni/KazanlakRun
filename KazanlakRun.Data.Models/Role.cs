using KazanlakRun.GCommon;
using System.ComponentModel.DataAnnotations;

namespace KazanlakRun.Data.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(
            ValidationConstants.RoleMaxLen,
            MinimumLength = ValidationConstants.RoleMinLen,
            ErrorMessage = "Role name must be between {2} and {1} characters long.")]
        public string Name { get; set; } = null!;

        public ICollection<VolunteerRole> VolunteerRoles { get; set; }
            = new List<VolunteerRole>();
    }
}
