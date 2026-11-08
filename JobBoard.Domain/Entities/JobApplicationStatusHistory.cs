using JobBoard.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Domain.Entities
{
    // One row per status change of a JobApplication. Rows are only ever added, never edited or
    // removed, so the full path an application took (and who moved it, and when) is never lost.
    // The starting "Applied" state is not a change: it is JobApplication.ApplicationDate.
    public class JobApplicationStatusHistory
    {
        public int Id { get; set; }
        public int JobApplicationId { get; set; }
        public ApplicationStatus FromStatus { get; set; }
        public ApplicationStatus ToStatus { get; set; }
        public DateTimeOffset ChangedAt { get; set; }

        // The user who made the change, taken from their JWT. Deliberately a plain column, not a
        // foreign key, so the audit trail doesn't depend on (or cascade from) the users table.
        [Required]
        public string ChangedByUserId { get; set; } = string.Empty;
    }
}
