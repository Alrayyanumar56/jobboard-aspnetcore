using System.ComponentModel.DataAnnotations;

namespace JobBoard.Domain.Entities
{
    // One education record on a candidate's profile. A profile can have many of them.
    // Skills, experience and projects are deliberately not part of this slice.
    public class CandidateEducation
    {
        public int Id { get; set; }

        // The profile this record belongs to (CandidateProfile.Id). Always set by the server from the
        // caller's OWN profile, never taken from a request.
        public int CandidateProfileId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Institution { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Degree { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string FieldOfStudy { get; set; } = string.Empty;

        // Dates only (no time of day): the database column is "date".
        public DateOnly StartDate { get; set; }

        // null means still studying.
        public DateOnly? EndDate { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }
    }
}
