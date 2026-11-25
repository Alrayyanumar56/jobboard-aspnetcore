using JobBoard.Application.DTOs;
using JobBoard.Domain.Entities;

namespace JobBoard.Application.EmployerProfiles
{
    // Shared by the three employer-profile handlers so the trimming rules and the mapping live in one place.
    internal static class EmployerProfileMappings
    {
        // Copies the editable fields from the command, trimmed; blank optional fields become null.
        // Never touches EmployerId, Id or the timestamps.
        public static void ApplyCommand(this EmployerProfile profile, EmployerProfileCommand command)
        {
            profile.CompanyName = command.CompanyName.Trim();
            profile.Website = NullIfBlank(command.Website);
            profile.About = NullIfBlank(command.About);
            profile.City = NullIfBlank(command.City);
        }

        public static EmployerProfileResponse ToResponse(this EmployerProfile profile)
        {
            return new EmployerProfileResponse
            {
                Id = profile.Id,
                CompanyName = profile.CompanyName,
                Website = profile.Website,
                About = profile.About,
                City = profile.City,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt
            };
        }

        private static string? NullIfBlank(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
