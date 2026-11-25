using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;
using JobBoard.Domain.Rules;
using JobBoard.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        // SQL Server error numbers: 2601 = duplicate key in a unique index,
        // 2627 = violation of a unique constraint.
        private const int UniqueIndexViolation = 2601;
        private const int UniqueConstraintViolation = 2627;

        private readonly ApplicationDbContext dbContext;

        public SkillRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Skill>> GetOrCreate(IReadOnlyCollection<string> cleanedNames)
        {
            if (cleanedNames.Count == 0)
            {
                return [];
            }

            // Normalized key -> the spelling to use if the skill has to be created.
            var wanted = cleanedNames.ToDictionary(SkillNameRules.Normalize, name => name);
            var keys = wanted.Keys.ToList();

            // Two attempts: if another request inserts one of the same new skills between our read and
            // our insert, the unique index rejects our insert; the second attempt then finds that row.
            for (var attempt = 1; ; attempt++)
            {
                var existing = await dbContext.Skill
                    .AsNoTracking()
                    .Where(s => keys.Contains(s.NormalizedName))
                    .ToListAsync();

                var missing = keys
                    .Except(existing.Select(s => s.NormalizedName))
                    .Select(key => new Skill { Name = wanted[key], NormalizedName = key })
                    .ToList();
                if (missing.Count == 0)
                {
                    return existing;
                }

                dbContext.Skill.AddRange(missing);
                try
                {
                    await dbContext.SaveChangesAsync();
                    return existing.Concat(missing).ToList();
                }
                catch (DbUpdateException ex) when (attempt == 1 && ex.InnerException is SqlException sqlEx
                    && (sqlEx.Number == UniqueIndexViolation || sqlEx.Number == UniqueConstraintViolation))
                {
                    // Forget the rejected inserts so they aren't tried again by the next SaveChanges.
                    foreach (var skill in missing)
                    {
                        dbContext.Entry(skill).State = EntityState.Detached;
                    }
                }
            }
        }

        public async Task<List<SkillResponse>> Search(string? search, int take)
        {
            var query = dbContext.Skill.AsNoTracking();

            var cleaned = SkillNameRules.Clean(search);
            if (cleaned != null)
            {
                // Compared on the normalized column, so "c#" finds "C#" whatever the collation.
                var key = SkillNameRules.Normalize(cleaned);
                query = query.Where(s => s.NormalizedName.Contains(key));
            }

            return await query
                .OrderBy(s => s.Name)
                .Take(take)
                .Select(s => new SkillResponse { Id = s.Id, Name = s.Name })
                .ToListAsync();
        }

        public async Task<List<string>> GetCandidateSkillNames(int candidateProfileId)
        {
            return await (from cs in dbContext.CandidateProfileSkill
                          join s in dbContext.Skill on cs.SkillId equals s.Id
                          where cs.CandidateProfileId == candidateProfileId
                          orderby s.Name
                          select s.Name)
                .ToListAsync();
        }
    }
}
