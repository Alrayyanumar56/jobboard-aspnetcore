namespace JobBoard.Application.DTOs
{
    // What an employer sees about the person behind one application on one of their jobs.
    // Deliberately has no JobSeekerId: the application id is the only handle the employer needs.
    public class ApplicantProfileResponse
    {
        public int ApplicationId { get; set; }

        // Null when the applicant hasn't created a candidate profile yet.
        public CandidateProfileResponse? Profile { get; set; }

        // Most recent first. Empty when they have no education records.
        public List<CandidateEducationResponse> Education { get; set; } = new();
    }
}
