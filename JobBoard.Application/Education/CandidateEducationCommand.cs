using System.ComponentModel.DataAnnotations;

namespace JobBoard.Application.Education
{
    // Request body for POST /api/CandidateEducation and PUT /api/CandidateEducation/{id}.
    // A full replacement of the editable fields. Deliberately has NO Id, CandidateProfileId or user id:
    // the client can't choose whose record this is.
    public class CandidateEducationCommand : IValidatableObject
    {
        // [Required] also rejects empty and whitespace-only text.
        [Required]
        [StringLength(200)]
        public string Institution { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Degree { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string FieldOfStudy { get; set; } = string.Empty;

        // Nullable ON PURPOSE: a plain DateOnly would silently become 0001-01-01 when the client forgets
        // the field, and [Required] would accept it. With DateOnly? a missing StartDate is a 400.
        [Required]
        public DateOnly? StartDate { get; set; }

        // Omitted or null means still studying.
        public DateOnly? EndDate { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        // Cross-field rule. ASP.NET Core runs this automatically ([ApiController]) and answers 400,
        // so the controller and handlers never see an inconsistent range.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate.HasValue && EndDate.HasValue && EndDate.Value < StartDate.Value)
            {
                yield return new ValidationResult(
                    "EndDate cannot be earlier than StartDate.",
                    new[] { nameof(EndDate) });
            }
        }
    }
}
