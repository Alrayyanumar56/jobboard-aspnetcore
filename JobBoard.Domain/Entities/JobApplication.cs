using JobBoard.Domain.Enums;
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

        // Current status and when it last changed. Only changed through
        // IJobApplicationRepository.TryChangeStatus, which also records a JobApplicationStatusHistory row.
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
        public DateTimeOffset StatusChangedAt { get; set; }
    }
}
