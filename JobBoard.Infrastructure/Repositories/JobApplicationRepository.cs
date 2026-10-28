using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;
using JobBoard.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        // SQL Server error numbers: 2601 = duplicate key in a unique index,
        // 2627 = violation of a unique constraint.
        private const int UniqueIndexViolation = 2601;
        private const int UniqueConstraintViolation = 2627;

        private readonly ApplicationDbContext dbContext;

        public JobApplicationRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> HasApplied(int jobId, string jobSeekerId)
        {
            return await dbContext.JobApplication
                .AsNoTracking()
                .AnyAsync(a => a.JobId == jobId && a.JobSeekerId == jobSeekerId);
        }

        public async Task<bool> TryAddApplication(JobApplication application)
        {
            dbContext.Add(application);
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
    }
}
