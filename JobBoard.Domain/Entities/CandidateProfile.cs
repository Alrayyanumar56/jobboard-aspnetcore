using System.ComponentModel.DataAnnotations;

namespace JobBoard.Domain.Entities
{
    // The core profile of one JobSeeker. At most one per JobSeeker: that rule is enforced by a unique
    // index on JobSeekerId in ApplicationDbContext. Education, skills, experience and projects are
    // deliberately not part of this entity.
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

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
