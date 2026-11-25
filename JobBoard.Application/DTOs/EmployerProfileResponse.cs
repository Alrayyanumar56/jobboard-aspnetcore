namespace JobBoard.Application.DTOs
{
    // An Employer's own company profile. Deliberately has no EmployerId: the caller is the owner.
    public class EmployerProfileResponse
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? About { get; set; }
        public string? City { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
