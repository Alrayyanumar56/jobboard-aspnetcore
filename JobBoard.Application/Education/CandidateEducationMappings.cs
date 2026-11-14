using JobBoard.Application.DTOs;
using JobBoard.Domain.Entities;

namespace JobBoard.Application.Education
{
    // Shared by the education handlers so the trimming rules and the mapping live in one place.
    internal static class CandidateEducationMappings
    {
        // Copies the editable fields from the command, trimmed; a blank Description becomes null.
        // Never touches Id or CandidateProfileId. The command has already passed validation, so
        // StartDate is known to have a value.
        public static void ApplyCommand(this CandidateEducation education, CandidateEducationCommand command)
        {
            education.Institution = command.Institution.Trim();
            education.Degree = command.Degree.Trim();
            education.FieldOfStudy = command.FieldOfStudy.Trim();
            education.StartDate = command.StartDate!.Value;
            education.EndDate = command.EndDate;
            education.Description = string.IsNullOrWhiteSpace(command.Description)
                ? null
                : command.Description.Trim();
        }

        public static CandidateEducationResponse ToResponse(this CandidateEducation education)
        {
            return new CandidateEducationResponse
            {
                Id = education.Id,
                Institution = education.Institution,
                Degree = education.Degree,
                FieldOfStudy = education.FieldOfStudy,
                StartDate = education.StartDate,
                EndDate = education.EndDate,
                Description = education.Description
            };
        }
    }
}
