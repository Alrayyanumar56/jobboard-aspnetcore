namespace JobBoard.Application.DTOs
{
    public class JobApplicationResponse
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobSeekerId { get; set; } = string.Empty;
        public DateTimeOffset ApplicationDate { get; set; }
    }
}
