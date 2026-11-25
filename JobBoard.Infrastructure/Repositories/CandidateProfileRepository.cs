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

        public async Task<bool> TryAddProfile(CandidateProfile profile, IReadOnlyCollection<int> skillIds)
        {
            // The profile row and its skill links must be saved together. The DbContext retries transient
            // errors, and a retrying strategy only allows our own transaction inside ExecuteAsync, which
            // replays the WHOLE block after a failure (same pattern as TryChangeStatus).
            var strategy = dbContext.Database.CreateExecutionStrategy();
            try
            {
                await strategy.ExecuteAsync(async () =>
                {
                    // A retry starts from a clean slate: forget whatever the failed attempt left tracked.
                    dbContext.ChangeTracker.Clear();
                    profile.Id = 0;

                    await using var transaction = await dbContext.Database.BeginTransactionAsync();

                    // First save gives the profile its identity Id, which the link rows need.
                    dbContext.CandidateProfile.Add(profile);
                    await dbContext.SaveChangesAsync();

                    dbContext.CandidateProfileSkill.AddRange(skillIds.Select(skillId =>
                        new CandidateProfileSkill { CandidateProfileId = profile.Id, SkillId = skillId }));
                    await dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                });
                return true;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx
                && (sqlEx.Number == UniqueIndexViolation || sqlEx.Number == UniqueConstraintViolation))
            {
                // Only the duplicate case is translated; any other database error still surfaces.
                // The transaction was rolled back when it was disposed without a commit.
                dbContext.ChangeTracker.Clear();
                return false;
            }
        }

        public async Task<bool> UpdateProfile(CandidateProfile profile, IReadOnlyCollection<int> skillIds)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                dbContext.ChangeTracker.Clear();
                await using var transaction = await dbContext.Database.BeginTransactionAsync();

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
                        .SetProperty(p => p.CvUrl, profile.CvUrl)
                        .SetProperty(p => p.YearsOfExperience, profile.YearsOfExperience)
                        .SetProperty(p => p.ExperienceLevel, profile.ExperienceLevel)
                        .SetProperty(p => p.UpdatedAt, profile.UpdatedAt));
                if (rows == 0)
                {
                    // Leaving without CommitAsync rolls the (empty) transaction back.
                    return false;
                }

                // Full replacement of the skill list. profile.Id came from the handler's read of THIS
                // owner's profile (JobSeekerId is unique), so these links can't touch anyone else's.
                await dbContext.CandidateProfileSkill.Where(cs => cs.CandidateProfileId == profile.Id).ExecuteDeleteAsync();
                dbContext.CandidateProfileSkill.AddRange(skillIds.Select(skillId =>
                    new CandidateProfileSkill { CandidateProfileId = profile.Id, SkillId = skillId }));
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            });
        }
    }
}
