using JobBoard.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Domain.Entities
{
    // The core profile of one JobSeeker. At most one per JobSeeker: that rule is enforced by a unique
    // index on JobSeekerId in ApplicationDbContext. Education (CandidateEducation) and skills
    // (CandidateProfileSkill) live in their own tables.
    public class CandidateProfile
    {
        public int Id { get; set; }

        // The owner: the JobSeeker's ApplicationUser Id, always taken from the JWT, never from a request.
        [Required]
        public string JobSeekerId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Headline { get; set; }

        [MaxLength(2000)]
        public string? Summary { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        // Link to the candidate's CV hosted elsewhere (http/https only). No file upload yet.
        [MaxLength(500)]
        public string? CvUrl { get; set; }

        public int? YearsOfExperience { get; set; }

        public ExperienceLevel? ExperienceLevel { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
