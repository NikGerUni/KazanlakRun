using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;
using KazanlakRun.GCommon;

namespace KazanlakRun.Areas.User.Models
{
    public class VolunteerInputModel : IValidatableObject
    {
        [Required(ErrorMessage = "Please enter your full name.")]
        [StringLength(ValidationConstants.NamesMaxLen,
            MinimumLength = ValidationConstants.NamesMinLen,
            ErrorMessage = "Name must be between {2} and {1} characters long.")]
        [RegularExpression(ValidationConstants.NamesRegex,
            ErrorMessage = "Enter exactly two names (Latin letters), e.g. “John Smith”.")]
        [Display(Name = "Two Names")]
        public string Names { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(ValidationConstants.EmailMaxLen,
            ErrorMessage = "Email cannot exceed {1} characters.")]
        [RegularExpression(ValidationConstants.EmailRegex,
            ErrorMessage = "Email must be in format name@domain.ext, no spaces, and TLD at least 2 letters.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(ValidationConstants.PhoneMaxLen,
            MinimumLength = ValidationConstants.PhoneMinLen,
            ErrorMessage = "Phone must be between {2} and {1} characters.")]
        [RegularExpression(ValidationConstants.PhoneRegex,
            ErrorMessage = "Invalid phone number format.")]
        [Display(Name = "Phone")]
        public string Phone { get; set; } = null!;

        // This method runs on the server after ModelBinding & DataAnnotations
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            // --- Advanced email checks ---
            if (!string.IsNullOrWhiteSpace(Email))
            {
                // 1. RFC email format via MailAddress
                try
                {
                    var mail = new MailAddress(Email);
                    if (!string.Equals(mail.Address, Email, StringComparison.Ordinal))
                    {
                        throw new FormatException();
                    }
                }
                catch
                {
                    errors.Add(new ValidationResult(
                        "Invalid email address format.",
                        new[] { nameof(Email) }));
                }

                // 2. No spaces
                if (Email.Contains(" "))
                {
                    errors.Add(new ValidationResult(
                        "Email cannot contain spaces.",
                        new[] { nameof(Email) }));
                }

                // 3. Domain must have at least one dot, TLD ≥2
                var atPos = Email.IndexOf('@');
                if (atPos < 0)
                {
                    errors.Add(new ValidationResult(
                        "Email must contain one '@' character.",
                        new[] { nameof(Email) }));
                }
                else
                {
                    var domain = Email[(atPos + 1)..];
                    if (!domain.Contains('.'))
                    {
                        errors.Add(new ValidationResult(
                            "Email domain must contain at least one dot (e.g. example.com).",
                            new[] { nameof(Email) }));
                    }
                    else
                    {
                        var tld = domain[(domain.LastIndexOf('.') + 1)..];
                        if (tld.Length < 2)
                        {
                            errors.Add(new ValidationResult(
                                "Top-level domain must be at least 2 characters long.",
                                new[] { nameof(Email) }));
                        }
                    }
                }
            }

            return errors;
        }
    }
}
