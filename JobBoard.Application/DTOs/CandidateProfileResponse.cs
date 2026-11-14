namespace JobBoard.Application.DTOs
{
    // A JobSeeker's own profile. Deliberately has no JobSeekerId: the caller is the owner.
    public class CandidateProfileResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Headline { get; set; }
        public string? Summary { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
