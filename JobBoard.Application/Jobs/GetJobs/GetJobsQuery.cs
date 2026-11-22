using JobBoard.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Application.Jobs.GetJobs
{
    // Query string of GET /api/Job?jobType=&location=&search=&page=&pageSize=
    // [ApiController] answers 400 when a value is out of range, so the handler only sees valid input.
    public class GetJobsQuery
    {
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 50;

        // Both filters are optional; null means "any".
        public JobType? JobType { get; set; }
        public Location? Location { get; set; }

        // Matches Title or Description. Blank means no search.
        [MaxLength(100)]
        public string? Search { get; set; }

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, MaxPageSize)]
        public int PageSize { get; set; } = DefaultPageSize;
    }
}
