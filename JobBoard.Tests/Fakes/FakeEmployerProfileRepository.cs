using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;

namespace JobBoard.Tests.Fakes
{
    // In-memory IEmployerProfileRepository: at most one profile per Employer, like the unique index.
    public class FakeEmployerProfileRepository : IEmployerProfileRepository
    {
        public List<EmployerProfile> Profiles { get; } = new();

        // Returns a COPY, like AsNoTracking: changing the result must not change the stored profile.
        public Task<EmployerProfile?> GetByEmployerId(string employerId)
        {
            var p = Profiles.FirstOrDefault(x => x.EmployerId == employerId);
            EmployerProfile? copy = p == null ? null : new EmployerProfile
            {
                Id = p.Id,
                EmployerId = p.EmployerId,
                CompanyName = p.CompanyName,
                Website = p.Website,
                About = p.About,
                City = p.City,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
            return Task.FromResult(copy);
        }

        public Task<bool> TryAddProfile(EmployerProfile profile)
        {
            if (Profiles.Any(p => p.EmployerId == profile.EmployerId))
            {
                return Task.FromResult(false);
            }
            profile.Id = Profiles.Count == 0 ? 1 : Profiles.Max(p => p.Id) + 1;
            Profiles.Add(profile);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateProfile(EmployerProfile profile)
        {
            var stored = Profiles.FirstOrDefault(p => p.EmployerId == profile.EmployerId);
            if (stored == null)
            {
                return Task.FromResult(false);
            }
            stored.CompanyName = profile.CompanyName;
            stored.Website = profile.Website;
            stored.About = profile.About;
            stored.City = profile.City;
            stored.UpdatedAt = profile.UpdatedAt;
            return Task.FromResult(true);
        }
    }
}
