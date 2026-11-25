using JobBoard.Domain.Enums;

namespace JobBoard.Application.DTOs
{
    // A JobSeeker's profile, as its owner (or an employer they applied to) sees it.
    // Deliberately has no JobSeekerId.
    public class CandidateProfileResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Headline { get; set; }
        public string? Summary { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? CvUrl { get; set; }
        public int? YearsOfExperience { get; set; }
        public ExperienceLevel? ExperienceLevel { get; set; }

        // Alphabetical. Empty when the profile lists no skills.
        public List<string> Skills { get; set; } = new();
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
