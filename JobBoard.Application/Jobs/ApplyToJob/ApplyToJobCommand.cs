using System.ComponentModel.DataAnnotations;

namespace JobBoard.Application.Jobs.ApplyToJob
{
    // Optional request body for POST /api/Job/{jobId}/applications. Sending no body at all is still
    // allowed. Deliberately has NO JobId or JobSeekerId: the URL and the token decide those.
    public class ApplyToJobCommand
    {
        [StringLength(4000)]
        public string? CoverLetter { get; set; }
    }
}
