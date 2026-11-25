using JobBoard.Application.Common;
using JobBoard.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Application.Profiles
{
    // Request body for POST and PUT /api/CandidateProfile. A full replacement of the editable fields.
    // Deliberately has NO JobSeekerId, Id or timestamps: the client can't choose whose profile this is.
    public class CandidateProfileCommand : IValidatableObject
    {
        // [Required] also rejects empty and whitespace-only text.
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        // Optional fields: omitted or blank clears them (this is a full replacement).
        [StringLength(150)]
        public string? Headline { get; set; }

        [StringLength(2000)]
        public string? Summary { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        // Link to a CV hosted elsewhere; http/https only.
        [StringLength(500)]
        public string? CvUrl { get; set; }

        [Range(0, 60)]
        public int? YearsOfExperience { get; set; }

        [EnumDataType(typeof(ExperienceLevel))]
        public ExperienceLevel? ExperienceLevel { get; set; }

        // Skill names, e.g. ["C#", "SQL"]. At most 20; blanks and repeats are ignored. Omitted clears them.
        public List<string>? Skills { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
            ValidationRules.ValidateHttpUrl(CvUrl, nameof(CvUrl))
                .Concat(ValidationRules.ValidateSkills(Skills, nameof(Skills)));
    }
}
