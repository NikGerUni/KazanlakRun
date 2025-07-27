using KazanlakRun.GCommon;
using System.ComponentModel.DataAnnotations;

namespace KazanlakRun.Web.Areas.Admin.Models
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Името е задължително.")]
        [StringLength(
        ValidationConstants.RoleMaxLen,
        MinimumLength = ValidationConstants.RoleMinLen,
        ErrorMessage = "Името трябва да е между {2} и {1} символа.")]
        public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}