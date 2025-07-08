// /Areas/User/Models/VolunteerInputModel.cs stays the same (your DTO-level checks)

// /Data/Models/Volunteer.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;
using KazanlakRun.GCommon;

namespace KazanlakRun.Data.Models
{
    public class Volunteer : IValidatableObject
    {
        public int Id { get; set; }
        public string? UserId { get; set; }

        /// <summary>Две имена</summary>
        public string Names { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        // Foreign key to AidStation
        public int? AidStationId { get; set; }
        public AidStation AidStation { get; set; } = null!;

        // Many-to-many to Role via join entity
        public ICollection<VolunteerRole> VolunteerRoles { get; set; }
            = new List<VolunteerRole>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // collect all errors
            var errors = new List<ValidationResult>();

            // --- Names: two Latin words, space separated, length bounds ---
            if (string.IsNullOrWhiteSpace(this.Names))
            {
                errors.Add(new ValidationResult(
                    "Names is required.",
                    new[] { nameof(this.Names) }));
            }
            else
            {
                if (!Regex.IsMatch(this.Names, ValidationConstatnts.NamesRegex))
                {
                    errors.Add(new ValidationResult(
                        "Enter exactly two names (Latin letters) separated by a single space.",
                        new[] { nameof(this.Names) }));
                }

                if (this.Names.Length < ValidationConstatnts.NamesMinLen
                    || this.Names.Length > ValidationConstatnts.NamesMaxLen)
                {
                    errors.Add(new ValidationResult(
                        $"Names must be between {ValidationConstatnts.NamesMinLen} and {ValidationConstatnts.NamesMaxLen} characters.",
                        new[] { nameof(this.Names) }));
                }
            }

            // --- Email ---
            // --- Разширена валидация за Email ---
            if (string.IsNullOrWhiteSpace(this.Email))
            {
                errors.Add(new ValidationResult(
                    "Email is required.",
                    new[] { nameof(this.Email) }));
            }
            else
            {
                // 1. Проверка на коректен формат с MailAddress
                try
                {
                    var mail = new MailAddress(this.Email);
                    if (mail.Address != this.Email)
                    {
                        throw new FormatException();
                    }
                }
                catch (FormatException)
                {
                    errors.Add(new ValidationResult(
                        "Invalid email address format.",
                        new[] { nameof(this.Email) }));
                }

                // 2. Дължина
                if (this.Email.Length > ValidationConstatnts.EmailMaxLen)
                {
                    errors.Add(new ValidationResult(
                        $"Email cannot exceed {ValidationConstatnts.EmailMaxLen} characters.",
                        new[] { nameof(this.Email) }));
                }

                // 3. Без интервали
                if (this.Email.Contains(" "))
                {
                    errors.Add(new ValidationResult(
                        "Email cannot contain spaces.",
                        new[] { nameof(this.Email) }));
                }

                // 4. Домейнът трябва да съдържа поне една точка
                var atIndex = this.Email.IndexOf('@');
                if (atIndex < 0)
                {
                    // вече хванато от MailAddress, но за по-ясна грешка:
                    errors.Add(new ValidationResult(
                        "Email must contain one ‘@’ character.",
                        new[] { nameof(this.Email) }));
                }
                else
                {
                    var domain = this.Email[(atIndex + 1)..];
                    if (!domain.Contains('.'))
                    {
                        errors.Add(new ValidationResult(
                            "Email domain must contain at least one dot (e.g. example.com).",
                            new[] { nameof(this.Email) }));
                    }
                    else
                    {
                        // проверка на TLD (най-малко 2 символа)
                        var tld = domain[(domain.LastIndexOf('.') + 1)..];
                        if (tld.Length < 2)
                        {
                            errors.Add(new ValidationResult(
                                "Top-level domain must be at least 2 characters long.",
                                new[] { nameof(this.Email) }));
                        }
                    }
                }
            }
            // --- Phone ---
            if (string.IsNullOrWhiteSpace(this.Phone))
            {
                errors.Add(new ValidationResult(
                    "Phone number is required.",
                    new[] { nameof(this.Phone) }));
            }
            else
            {
                if (!Regex.IsMatch(this.Phone, ValidationConstatnts.PhoneRegex))
                {
                    errors.Add(new ValidationResult(
                        "Invalid phone number format.",
                        new[] { nameof(this.Phone) }));
                }
                if (this.Phone.Length < ValidationConstatnts.PhoneMinLen
                    || this.Phone.Length > ValidationConstatnts.PhoneMaxLen)
                {
                    errors.Add(new ValidationResult(
                        $"Phone must be between {ValidationConstatnts.PhoneMinLen} and {ValidationConstatnts.PhoneMaxLen} characters.",
                        new[] { nameof(this.Phone) }));
                }
            }

            return errors;
        }
    }
}


