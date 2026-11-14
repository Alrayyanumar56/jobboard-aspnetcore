namespace JobBoard.Application.DTOs
{
    // One of the caller's own education records. Deliberately has no profile id or user id:
    // the caller is the owner.
    public class CandidateEducationResponse
    {
        public int Id { get; set; }
        public string Institution { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string FieldOfStudy { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }
    }
}
