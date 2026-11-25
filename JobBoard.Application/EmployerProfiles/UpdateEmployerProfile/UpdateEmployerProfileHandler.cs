using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;

namespace JobBoard.Application.EmployerProfiles.UpdateEmployerProfile
{
    public class UpdateEmployerProfileHandler
    {
        private readonly IEmployerProfileRepository employerProfileRepository;

        public UpdateEmployerProfileHandler(IEmployerProfileRepository employerProfileRepository)
        {
            this.employerProfileRepository = employerProfileRepository;
        }

        // employerId comes from the authenticated user's JWT (resolved in the controller), never from
        // the request body. The profile is looked up BY that id, so ownership is built into the lookup.
        // Returns null when they have no profile yet; the controller turns that into a 404.
        public async Task<EmployerProfileResponse?> Handle(EmployerProfileCommand command, string employerId)
        {
            var profile = await employerProfileRepository.GetByEmployerId(employerId);
            if (profile == null)
            {
                return null;
            }

            // GetByEmployerId is AsNoTracking, so this object is only a carrier of the new values.
            // EmployerId, Id and CreatedAt are never taken from the command.
            profile.ApplyCommand(command);
            profile.UpdatedAt = DateTimeOffset.UtcNow;

            // The repository's UPDATE is keyed by EmployerId too. False means the row vanished
            // after we read it.
            if (!await employerProfileRepository.UpdateProfile(profile))
            {
                return null;
            }

            return profile.ToResponse();
        }
    }
}
