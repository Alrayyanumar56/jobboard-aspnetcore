using JobBoard.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Application.Jobs.UpdateJob
{
    // Request body for PUT /api/Job/{jobId}. A full replacement of the editable fields, same rules
    // as CreateJobCommand. Deliberately has NO EmployerId, PostedAt or ClosedAt: the client can't
    // change who owns a job, when it was posted, or whether it is closed.
    public class UpdateJobCommand
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        // Nullable + [Required] so an omitted field is a 400, not a silent default of FullTime (0).
        // [EnumDataType] rejects undefined numbers such as 99.
        [Required]
        [EnumDataType(typeof(JobType))]
        public JobType? JobType { get; set; }

        [Required]
        [EnumDataType(typeof(Location))]
        public Location? JobLocation { get; set; }

        // Omitted or blank clears the salary range (this is a full replacement).
        [StringLength(100)]
        public string? SalaryRange { get; set; }
    }
}
