using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;
using JobBoard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.Repositories
{
    public class CandidateEducationRepository : ICandidateEducationRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CandidateEducationRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // THE ownership rule, in one place: an education record is the caller's only if it sits on a
        // profile whose JobSeekerId is the caller's. Every read, update and delete below starts from this
        // query, so none of them can reach another candidate's record. It becomes an EXISTS subquery in SQL.
        private IQueryable<CandidateEducation> OwnedBy(string jobSeekerId)
        {
            return dbContext.CandidateEducation.Where(e =>
                dbContext.CandidateProfile.Any(p => p.Id == e.CandidateProfileId && p.JobSeekerId == jobSeekerId));
        }

        public async Task<List<CandidateEducation>> GetAllForJobSeeker(string jobSeekerId)
        {
            // Most recent first, the usual CV order. Id is the tiebreaker so the order is always stable.
            return await OwnedBy(jobSeekerId)
                .AsNoTracking()
                .OrderByDescending(e => e.StartDate)
                .ThenByDescending(e => e.Id)
                .ToListAsync();
        }

        public async Task<CandidateEducation?> GetByIdForJobSeeker(string jobSeekerId, int educationId)
        {
            return await OwnedBy(jobSeekerId)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == educationId);
        }

        public async Task AddEducation(CandidateEducation education)
        {
            dbContext.Add(education);
            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateEducation(string jobSeekerId, CandidateEducation education)
        {
            // One atomic UPDATE. WHERE = this record AND owned by the caller. Only the editable columns
            // are SET: Id and CandidateProfileId can't change.
            var rows = await OwnedBy(jobSeekerId)
                .Where(e => e.Id == education.Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(e => e.Institution, education.Institution)
                    .SetProperty(e => e.Degree, education.Degree)
                    .SetProperty(e => e.FieldOfStudy, education.FieldOfStudy)
                    .SetProperty(e => e.StartDate, education.StartDate)
                    .SetProperty(e => e.EndDate, education.EndDate)
                    .SetProperty(e => e.Description, education.Description));
            return rows > 0;
        }

        public async Task<bool> DeleteEducation(string jobSeekerId, int educationId)
        {
            // One atomic DELETE. WHERE = this record AND owned by the caller.
            var rows = await OwnedBy(jobSeekerId)
                .Where(e => e.Id == educationId)
                .ExecuteDeleteAsync();
            return rows > 0;
        }
    }
}
