using JobBoard.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Application.Jobs.UpdateApplicationStatus
{
    // Request body for PATCH /api/JobApplication/{applicationId}/status.
    // Deliberately has NO employer or applicant id: who is calling comes only from the JWT.
    public class UpdateApplicationStatusCommand
    {
        // Nullable + [Required] so an omitted field is a 400, not a silent default of Applied (0).
        // [EnumDataType] rejects undefined numbers such as 99.
        [Required]
        [EnumDataType(typeof(ApplicationStatus))]
        public ApplicationStatus? Status { get; set; }
    }
}
