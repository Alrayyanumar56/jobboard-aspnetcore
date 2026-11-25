using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;

namespace JobBoard.Application.Jobs.GetJobs
{
    public class GetJobsHandler
    {
        private readonly IJobRepository jobRepository;

        public GetJobsHandler(IJobRepository jobRepository)
        {
            this.jobRepository = jobRepository;
        }

        // Public listing: one page of open jobs whose deadline (if any) hasn't passed, newest first.
        public async Task<PagedResult<JobResponse>> Handle(GetJobsQuery query)
        {
            var search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim();

            var (jobs, totalCount) = await jobRepository.GetOpenJobsPage(query.JobType, query.Location, search, query.Page, query.PageSize, DateTimeOffset.UtcNow);

            return new PagedResult<JobResponse>
            {
                Items = jobs,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }
    }
}
