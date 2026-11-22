using JobBoard.Domain.Enums;

namespace JobBoard.Application.DTOs
{
    // One row of "my applications" for the logged-in JobSeeker.
    // Deliberately has no JobSeekerId: every row belongs to the caller already.
    public class MyApplicationResponse
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;

        // True when the employer has closed the job. A closed job is no longer served by
        // GET /api/Job/{id}, so a frontend uses this to show "closed" instead of linking to a 404.
        public bool JobIsClosed { get; set; }
        public DateTimeOffset ApplicationDate { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateTimeOffset StatusChangedAt { get; set; }
    }
}
