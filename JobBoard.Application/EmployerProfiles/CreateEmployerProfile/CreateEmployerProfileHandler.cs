using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;

namespace JobBoard.Application.EmployerProfiles.CreateEmployerProfile
{
    public class CreateEmployerProfileHandler
    {
        private readonly IEmployerProfileRepository employerProfileRepository;

        public CreateEmployerProfileHandler(IEmployerProfileRepository employerProfileRepository)
        {
            this.employerProfileRepository = employerProfileRepository;
        }

        // employerId comes from the authenticated user's JWT (resolved in the controller), never from
        // the request body. Returns null when this Employer already has a profile; the controller
        // turns that into a 409.
        public async Task<EmployerProfileResponse?> Handle(EmployerProfileCommand command, string employerId)
        {
            // Friendly early check for the normal "already created" case.
            if (await employerProfileRepository.GetByEmployerId(employerId) != null)
            {
                return null;
            }

            var now = DateTimeOffset.UtcNow;
            var profile = new EmployerProfile
            {
                EmployerId = employerId,
                CreatedAt = now,
                UpdatedAt = now
            };
            profile.ApplyCommand(command);

            // Two simultaneous requests can both pass the check above. The unique index on EmployerId
            // is the real guarantee: it rejects the second insert and TryAddProfile reports false.
            if (!await employerProfileRepository.TryAddProfile(profile))
            {
                return null;
            }

            return profile.ToResponse();
        }
    }
}
