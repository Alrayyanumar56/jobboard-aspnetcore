using System.ComponentModel.DataAnnotations;

namespace JobBoard.Domain.Entities
{
    // The company behind one Employer account. At most one per Employer: enforced by a unique index on
    // EmployerId in ApplicationDbContext. Its CompanyName is shown on the employer's job postings.
    public class EmployerProfile
    {
        public int Id { get; set; }

        // The owner: the Employer's ApplicationUser Id, always taken from the JWT, never from a request.
        [Required]
        public string EmployerId { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        // http/https only (checked by the API).
        [MaxLength(200)]
        public string? Website { get; set; }

        [MaxLength(2000)]
        public string? About { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
