namespace JobBoard.Application.DTOs
{
    // One row of "my applications" for the logged-in JobSeeker.
    // Deliberately has no JobSeekerId: every row belongs to the caller already.
    public class MyApplicationResponse
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public DateTimeOffset ApplicationDate { get; set; }
    }
}
