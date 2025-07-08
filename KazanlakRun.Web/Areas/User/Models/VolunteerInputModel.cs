using System.ComponentModel.DataAnnotations;
using KazanlakRun.GCommon;

namespace KazanlakRun.Areas.User.Models
{
    public class VolunteerInputModel
    {
        [Required(ErrorMessage = "Please enter your full name.")]
        [StringLength(ValidationConstatnts.NamesMaxLen,
                      MinimumLength = ValidationConstatnts.NamesMinLen,
                      ErrorMessage = "Name must be between {2} and {1} characters long.")]
        [RegularExpression(ValidationConstatnts.NamesRegex,
            ErrorMessage = "Enter exactly two names (Latin letters), e.g. “John Smith”.")]
        public string Names { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(ValidationConstatnts.EmailMaxLen,
                      ErrorMessage = "Email cannot exceed {1} characters.")]
        [RegularExpression(
            ValidationConstatnts.EmailRegex,
            ErrorMessage = "Email must be in format name@domain.ext, no spaces, and TLD at least 2 letters.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(ValidationConstatnts.PhoneRegex,
            ErrorMessage = "Invalid phone number format.")]
        [StringLength(ValidationConstatnts.PhoneMaxLen,
                      MinimumLength = ValidationConstatnts.PhoneMinLen,
                      ErrorMessage = "Phone must be between {2} and {1} characters.")]
        public string Phone { get; set; } = null!;
    }
}
