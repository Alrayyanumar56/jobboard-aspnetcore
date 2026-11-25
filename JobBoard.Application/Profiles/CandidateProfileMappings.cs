using JobBoard.Application.DTOs;
using JobBoard.Domain.Entities;

namespace JobBoard.Application.Profiles
{
    // Shared by the three profile handlers so the trimming rules and the mapping live in one place.
    internal static class CandidateProfileMappings
    {
        // Copies the editable fields from the command, trimmed; blank optional fields become null.
        // Never touches JobSeekerId, Id or the timestamps.
        public static void ApplyCommand(this CandidateProfile profile, CandidateProfileCommand command)
        {
            profile.FullName = command.FullName.Trim();
            profile.Headline = NullIfBlank(command.Headline);
            profile.Summary = NullIfBlank(command.Summary);
            profile.City = NullIfBlank(command.City);
            profile.Country = NullIfBlank(command.Country);
            profile.CvUrl = NullIfBlank(command.CvUrl);
            profile.YearsOfExperience = command.YearsOfExperience;
            profile.ExperienceLevel = command.ExperienceLevel;
        }

        // skills: the profile's skill names, alphabetical (read separately: they live in their own table).
        public static CandidateProfileResponse ToResponse(this CandidateProfile profile, List<string> skills)
        {
            return new CandidateProfileResponse
            {
                Id = profile.Id,
                FullName = profile.FullName,
                Headline = profile.Headline,
                Summary = profile.Summary,
                City = profile.City,
                Country = profile.Country,
                CvUrl = profile.CvUrl,
                YearsOfExperience = profile.YearsOfExperience,
                ExperienceLevel = profile.ExperienceLevel,
                Skills = skills,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt
            };
        }

        private static string? NullIfBlank(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
