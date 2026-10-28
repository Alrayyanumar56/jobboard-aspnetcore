using System.ComponentModel.DataAnnotations;

namespace JobBoard.Domain.Entities
{
    // One JobSeeker applying to one Job. The (JobId, JobSeekerId) pair is unique;
    // that rule is enforced by a unique index in ApplicationDbContext.
    public class JobApplication
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        [Required]
        public string JobSeekerId { get; set; } = string.Empty;
        public DateTimeOffset ApplicationDate { get; set; }
    }
}
