using System.ComponentModel.DataAnnotations;

namespace JobBoard.Application.Profiles
{
    // Request body for POST and PUT /api/CandidateProfile. A full replacement of the editable fields.
    // Deliberately has NO JobSeekerId, Id or timestamps: the client can't choose whose profile this is.
    public class CandidateProfileCommand
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
    }
}
