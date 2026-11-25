using JobBoard.Domain.Enums;

namespace JobBoard.Application.DTOs
{
    // One row of GET /api/Job/mine: a job the calling employer posted, open or closed.
    // No EmployerId: every row belongs to the caller already.
    public class MyJobResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public JobType JobType { get; set; }
        public Location JobLocation { get; set; }
        public string? SalaryRange { get; set; }
        public DateTimeOffset PostedAt { get; set; }
        public string? City { get; set; }
        public ExperienceLevel? ExperienceLevel { get; set; }
        public DateTimeOffset? ApplicationDeadline { get; set; }

        // Alphabetical. Empty when the job lists no skills.
        public List<string> Skills { get; set; } = new();

        // Null while the job is open.
        public DateTimeOffset? ClosedAt { get; set; }

        // How many applications the job has received (all statuses, including withdrawn).
        public int ApplicationCount { get; set; }
    }
}
