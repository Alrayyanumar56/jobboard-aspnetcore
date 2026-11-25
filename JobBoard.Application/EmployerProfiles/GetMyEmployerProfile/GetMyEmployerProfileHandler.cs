using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;

namespace JobBoard.Application.EmployerProfiles.GetMyEmployerProfile
{
    public class GetMyEmployerProfileHandler
    {
        private readonly IEmployerProfileRepository employerProfileRepository;

        public GetMyEmployerProfileHandler(IEmployerProfileRepository employerProfileRepository)
        {
            this.employerProfileRepository = employerProfileRepository;
        }

        // employerId comes from the authenticated user's JWT (resolved in the controller), never from
        // the request. The lookup is keyed by it, so an Employer can only ever read their own profile.
        // Returns null when they haven't created one yet; the controller turns that into a 404.
        public async Task<EmployerProfileResponse?> Handle(string employerId)
        {
            var profile = await employerProfileRepository.GetByEmployerId(employerId);
            return profile?.ToResponse();
        }
    }
}
