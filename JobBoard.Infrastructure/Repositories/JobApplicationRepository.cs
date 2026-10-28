using JobBoard.Application.DTOs;
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
        public async Task<List<MyApplicationResponse>> GetApplicationsByJobSeeker(string jobSeekerId)
        {
            // One query: join to Job for the title and select only the four columns the response needs.
            // A projection to a DTO is never tracked, so no entities are loaded.
            var query = from a in dbContext.JobApplication
                        join j in dbContext.Job on a.JobId equals j.Id
                        where a.JobSeekerId == jobSeekerId
                        orderby a.ApplicationDate descending
                        select new MyApplicationResponse
                        {
                            Id = a.Id,
                            JobId = a.JobId,
                            JobTitle = j.Title,
                            ApplicationDate = a.ApplicationDate
                        };

            return await query.ToListAsync();
        }
    }
}
