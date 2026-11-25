using JobBoard.Application.Common;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Application.EmployerProfiles
{
    // Request body for POST /api/EmployerProfile and PUT /api/EmployerProfile/me. A full replacement of
    // the editable fields. Deliberately has NO EmployerId, Id or timestamps: the client can't choose
    // whose company profile this is.
    public class EmployerProfileCommand : IValidatableObject
    {
        // [Required] also rejects empty and whitespace-only text.
        [Required]
        [StringLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        // Optional fields: omitted or blank clears them (this is a full replacement).
        [StringLength(200)]
        public string? Website { get; set; }

        [StringLength(2000)]
        public string? About { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
            ValidationRules.ValidateHttpUrl(Website, nameof(Website));
    }
}
