using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;
using JobBoard.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.Repositories
{
    public class EmployerProfileRepository : IEmployerProfileRepository
    {
        // SQL Server error numbers: 2601 = duplicate key in a unique index,
        // 2627 = violation of a unique constraint.
        private const int UniqueIndexViolation = 2601;
        private const int UniqueConstraintViolation = 2627;

        private readonly ApplicationDbContext dbContext;

        public EmployerProfileRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<EmployerProfile?> GetByEmployerId(string employerId)
        {
            return await dbContext.EmployerProfile
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.EmployerId == employerId);
        }

        public async Task<bool> TryAddProfile(EmployerProfile profile)
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
                dbContext.Entry(profile).State = EntityState.Detached;
                return false;
            }
        }

        public async Task<bool> UpdateProfile(EmployerProfile profile)
        {
            // One atomic UPDATE keyed by the owner's id. Only the editable columns (and UpdatedAt) are
            // SET: EmployerId, Id and CreatedAt can't change.
            var rows = await dbContext.EmployerProfile
                .Where(p => p.EmployerId == profile.EmployerId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.CompanyName, profile.CompanyName)
                    .SetProperty(p => p.Website, profile.Website)
                    .SetProperty(p => p.About, profile.About)
                    .SetProperty(p => p.City, profile.City)
                    .SetProperty(p => p.UpdatedAt, profile.UpdatedAt));
            return rows > 0;
        }
    }
}
