using JobBoard.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace JobBoard.Domain.Entities
{
    public class Job
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100,ErrorMessage ="Title should be under 100 characters")]
        public String Title { get; set; }
        [Required]
        [MaxLength(1000,ErrorMessage ="Description should be under 1000 characters")]
        public String Description { get; set; }
        public JobType JobType { get; set; }
        public Location JobLocation { get; set; }
        [Required]
        public String EmployerId { get; set; }
        [Required]
        public DateTimeOffset PostedAt{ get; set; }
        public String? SalaryRange { get; set; }

        // The city the job is in (or near, for Hybrid). Optional: a Remote job may have none.
        [MaxLength(100)]
        public string? City { get; set; }

        // Null only for jobs created before this field existed; new and edited jobs must set it.
        public ExperienceLevel? ExperienceLevel { get; set; }

        // null = no deadline. After this moment the job is hidden from the public listing and
        // accepts no new applications (it can still be opened by id).
        public DateTimeOffset? ApplicationDeadline { get; set; }
        // null = open. Set when the employer closes the job. A closed job is hidden from public
        // browsing and accepts no new applications, but the row (and its applications) is kept.
        public DateTimeOffset? ClosedAt { get; set; }
    }
}
