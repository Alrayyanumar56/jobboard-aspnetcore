using JobBoard.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JobBoard.Application.Jobs.CreateJob
{
    // Request body for POST /api/Job.
    // Deliberately has NO EmployerId or PostedAt: the server decides both.
    public class CreateJobCommand
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

        [StringLength(100)]
        public string? SalaryRange { get; set; }
    }
}
