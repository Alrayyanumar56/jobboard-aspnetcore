using JobBoard.Domain.Enums;

namespace JobBoard.Application.DTOs
{
    // One status change in an application's history. ChangedByUserId is returned as a plain id on
    // purpose: the front end resolves it to a name/details itself, so no user data is exposed here.
    public class ApplicationStatusHistoryResponse
    {
        public ApplicationStatus FromStatus { get; set; }
        public ApplicationStatus ToStatus { get; set; }
        public DateTimeOffset ChangedAt { get; set; }
        public string ChangedByUserId { get; set; } = string.Empty;
    }
}
