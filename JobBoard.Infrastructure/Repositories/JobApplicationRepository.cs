using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;
using JobBoard.Domain.Enums;
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
        public async Task<List<JobApplicantResponse>> GetApplicantsByJob(int jobId)
        {
            // One query: join to the Identity users table, but select ONLY the email from it,
            // so no password hash or other Identity column is ever read.
            var query = from a in dbContext.JobApplication
                        join u in dbContext.Users on a.JobSeekerId equals u.Id
                        where a.JobId == jobId
                        orderby a.ApplicationDate descending
                        select new JobApplicantResponse
                        {
                            Id = a.Id,
                            JobId = a.JobId,
                            ApplicantEmail = u.Email ?? string.Empty,
                            ApplicationDate = a.ApplicationDate,
                            Status = a.Status,
                            StatusChangedAt = a.StatusChangedAt
                        };

            return await query.ToListAsync();
        }

        public async Task<List<MyApplicationResponse>> GetApplicationsByJobSeeker(string jobSeekerId)
        {
            // One query: join to Job for the title and select only the columns the response needs.
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
                            ApplicationDate = a.ApplicationDate,
                            Status = a.Status,
                            StatusChangedAt = a.StatusChangedAt
                        };

            return await query.ToListAsync();
        }

        public async Task<JobApplication?> GetApplicationById(int applicationId)
        {
            return await dbContext.JobApplication
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == applicationId);
        }

        public async Task<bool> TryChangeStatus(int applicationId, ApplicationStatus from, ApplicationStatus to, string changedByUserId, DateTimeOffset changedAt)
        {
            // The status change and its history row must succeed or fail together, so they share a transaction.
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            // Compare-and-swap: the UPDATE only matches while the row is STILL in `from`. If two requests
            // race, one matches and the other gets 0 rows, so a decision made on stale data is never applied.
            var rows = await dbContext.JobApplication
                .Where(a => a.Id == applicationId && a.Status == from)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(a => a.Status, to)
                    .SetProperty(a => a.StatusChangedAt, changedAt));

            if (rows == 0)
            {
                // Leaving without CommitAsync rolls the (empty) transaction back.
                return false;
            }

            dbContext.JobApplicationStatusHistory.Add(new JobApplicationStatusHistory
            {
                JobApplicationId = applicationId,
                FromStatus = from,
                ToStatus = to,
                ChangedAt = changedAt,
                ChangedByUserId = changedByUserId
            });
            await dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }

        public async Task<List<ApplicationStatusHistoryResponse>> GetStatusHistory(int applicationId)
        {
            // Read-only projection straight to the DTO. Id is the tiebreaker: two changes can share a
            // ChangedAt, but Id only ever increases, so the order is always stable.
            return await dbContext.JobApplicationStatusHistory
                .AsNoTracking()
                .Where(h => h.JobApplicationId == applicationId)
                .OrderBy(h => h.ChangedAt)
                .ThenBy(h => h.Id)
                .Select(h => new ApplicationStatusHistoryResponse
                {
                    FromStatus = h.FromStatus,
                    ToStatus = h.ToStatus,
                    ChangedAt = h.ChangedAt,
                    ChangedByUserId = h.ChangedByUserId
                })
                .ToListAsync();
        }
    }
}
