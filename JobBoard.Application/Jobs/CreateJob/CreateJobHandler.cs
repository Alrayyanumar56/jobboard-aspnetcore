using JobBoard.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JobBoard.Application.Jobs.CreateJob
{
    // Request body for POST /api/Job.
    // Deliberately has NO EmployerId or PostedAt: the server decides both.
    public class CreateJobCommand : IValidatableObject
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

        [StringLength(100)]
        public string? City { get; set; }

        // Required for every new or edited job. [EnumDataType] rejects undefined numbers such as 99.
        [Required]
        [EnumDataType(typeof(ExperienceLevel))]
        public ExperienceLevel? ExperienceLevel { get; set; }

        // Optional; must be in the future when given. Omitted or null means no deadline.
        public DateTimeOffset? ApplicationDeadline { get; set; }

        // Skill names, e.g. ["C#", "SQL"]. At most 20; blanks and repeats are ignored.
        public List<string>? Skills { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
            JobCommandRules.Validate(ApplicationDeadline, Skills);
    }
}
