using JobBoard.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobBoard.Application.DTOs
{
    public class JobResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public JobType JobType { get; set; }
        public Location JobLocation { get; set; }
        public string? SalaryRange { get; set; }
        public string EmployerId { get; set; } = string.Empty;
        public DateTimeOffset PostedAt { get; set; }
        public string? City { get; set; }

        // Null only for jobs created before experience levels existed.
        public ExperienceLevel? ExperienceLevel { get; set; }

        // Null = no deadline.
        public DateTimeOffset? ApplicationDeadline { get; set; }

        // From the employer's company profile. Null while they haven't created one.
        public string? CompanyName { get; set; }

        // Alphabetical. Empty when the job lists no skills.
        public List<string> Skills { get; set; } = new();
    }
}
