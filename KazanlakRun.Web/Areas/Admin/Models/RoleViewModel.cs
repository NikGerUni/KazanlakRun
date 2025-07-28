using KazanlakRun.GCommon;
using System.ComponentModel.DataAnnotations;

namespace KazanlakRun.Web.Areas.Admin.Models
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(
            ValidationConstants.RoleMaxLen,
            MinimumLength = ValidationConstants.RoleMinLen,
            ErrorMessage = "Name must be between {2} and {1} characters long.")]
        public string Name { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
    }
}
