using JobBoard.Domain.Enums;

namespace JobBoard.Application.DTOs
{
    // The result of a status change. Deliberately has no JobSeekerId or email: the caller already
    // knows which application they just changed.
    public class ApplicationStatusResponse
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateTimeOffset StatusChangedAt { get; set; }
    }
}
