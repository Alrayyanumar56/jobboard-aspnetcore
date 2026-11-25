using JobBoard.Domain.Entities;

namespace JobBoard.Application.Interfaces
{
    public interface IEmployerProfileRepository
    {
        // The company profile owned by this Employer, or null if they haven't created one yet.
        public Task<EmployerProfile?> GetByEmployerId(string employerId);

        // Returns false (instead of throwing) when the unique index on EmployerId rejects a second profile.
        public Task<bool> TryAddProfile(EmployerProfile profile);

        // Writes CompanyName, Website, About, City and UpdatedAt in one UPDATE whose WHERE is the
        // profile's EmployerId. EmployerId and CreatedAt can't change. False means no row matched.
        public Task<bool> UpdateProfile(EmployerProfile profile);
    }
}
