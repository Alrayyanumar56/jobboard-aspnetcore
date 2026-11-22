using JobBoard.Domain.Enums;

namespace JobBoard.Application.DTOs
{
    // One applicant on one of the employer's jobs.
    // Deliberately limited to what an employer needs: no user ID, no Identity fields beyond email.
    public class JobApplicantResponse
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string ApplicantEmail { get; set; } = string.Empty;

        // From the applicant's candidate profile. Null when they haven't created a profile yet.
        public string? ApplicantName { get; set; }
        public DateTimeOffset ApplicationDate { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateTimeOffset StatusChangedAt { get; set; }
    }
}
