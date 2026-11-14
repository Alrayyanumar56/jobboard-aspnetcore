using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;
using JobBoard.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.Repositories
{
    public class CandidateProfileRepository : ICandidateProfileRepository
    {
        // SQL Server error numbers: 2601 = duplicate key in a unique index,
        // 2627 = violation of a unique constraint.
        private const int UniqueIndexViolation = 2601;
        private const int UniqueConstraintViolation = 2627;

        private readonly ApplicationDbContext dbContext;

        public CandidateProfileRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<CandidateProfile?> GetByJobSeekerId(string jobSeekerId)
        {
            return await dbContext.CandidateProfile
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.JobSeekerId == jobSeekerId);
        }

        public async Task<bool> TryAddProfile(CandidateProfile profile)
        {
            dbContext.Add(profile);
            try
            {
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx
                && (sqlEx.Number == UniqueIndexViolation || sqlEx.Number == UniqueConstraintViolation))
            {
                // Only the duplicate case is translated; any other database error still surfaces.
                return false;
            }
        }

        public async Task<bool> UpdateProfile(CandidateProfile profile)
        {
            // One atomic UPDATE keyed by the owner's id. Only the editable columns (and UpdatedAt) are
            // SET: JobSeekerId, Id and CreatedAt can't change.
            var rows = await dbContext.CandidateProfile
                .Where(p => p.JobSeekerId == profile.JobSeekerId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.FullName, profile.FullName)
                    .SetProperty(p => p.Headline, profile.Headline)
                    .SetProperty(p => p.Summary, profile.Summary)
                    .SetProperty(p => p.City, profile.City)
                    .SetProperty(p => p.Country, profile.Country)
                    .SetProperty(p => p.UpdatedAt, profile.UpdatedAt));
            return rows > 0;
        }
    }
}
