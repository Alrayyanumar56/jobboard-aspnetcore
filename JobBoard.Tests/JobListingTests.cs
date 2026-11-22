using JobBoard.Application.Jobs.CloseJob;
using JobBoard.Application.Jobs.GetJobs;
using JobBoard.Application.Jobs.GetMyJobs;
using JobBoard.Domain.Entities;
using JobBoard.Domain.Enums;
using JobBoard.Tests.Fakes;

namespace JobBoard.Tests
{
    // GET /api/Job (paging + search) and GET /api/Job/mine?status=...
    public class JobListingTests
    {
        private const string EmployerA = "employer-a";
        private const string EmployerB = "employer-b";

        private readonly FakeJobRepository jobs = new();

        private Task<Job> Post(string title, string employerId = EmployerA, int minutesAgo = 0) =>
            jobs.AddJob(new Job
            {
                Title = title,
                Description = "Description of " + title,
                JobType = JobType.FullTime,
                JobLocation = Location.OnSite,
                EmployerId = employerId,
                PostedAt = DateTimeOffset.UtcNow.AddMinutes(-minutesAgo)
            });

        [Fact]
        public async Task Listing_returns_requested_page_and_total_count()
        {
            for (var i = 0; i < 5; i++)
            {
                await Post($"Job {i}", minutesAgo: i);
            }

            var result = await new GetJobsHandler(jobs).Handle(new GetJobsQuery { Page = 2, PageSize = 2 });

            Assert.Equal(5, result.TotalCount);
            Assert.Equal(2, result.Page);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(["Job 2", "Job 3"], result.Items.Select(j => j.Title));
        }

        [Fact]
        public async Task Listing_hides_closed_jobs_from_items_and_total()
        {
            var closed = await Post("Closed job");
            await Post("Open job");
            await new CloseJobHandler(jobs).Handle(closed.Id, EmployerA);

            var result = await new GetJobsHandler(jobs).Handle(new GetJobsQuery());

            Assert.Equal(1, result.TotalCount);
            Assert.Equal("Open job", Assert.Single(result.Items).Title);
        }

        [Fact]
        public async Task Search_is_trimmed_and_matches_title()
        {
            await Post("Senior React Developer");
            await Post("Accountant");

            var result = await new GetJobsHandler(jobs).Handle(new GetJobsQuery { Search = "  react  " });

            Assert.Equal("Senior React Developer", Assert.Single(result.Items).Title);
        }

        [Fact]
        public async Task Blank_search_returns_everything()
        {
            await Post("A");
            await Post("B");

            var result = await new GetJobsHandler(jobs).Handle(new GetJobsQuery { Search = "   " });

            Assert.Equal(2, result.TotalCount);
        }

        [Theory]
        [InlineData(JobStatusFilter.All, new[] { "Open", "Closed" })]
        [InlineData(JobStatusFilter.Open, new[] { "Open" })]
        [InlineData(JobStatusFilter.Closed, new[] { "Closed" })]
        public async Task My_jobs_filters_by_status(JobStatusFilter status, string[] expectedTitles)
        {
            var closed = await Post("Closed", minutesAgo: 10);
            await Post("Open");
            await new CloseJobHandler(jobs).Handle(closed.Id, EmployerA);

            var result = await new GetMyJobsHandler(jobs).Handle(EmployerA, status);

            Assert.Equal(expectedTitles, result.Select(j => j.Title));
        }

        [Fact]
        public async Task My_jobs_never_includes_another_employers_jobs()
        {
            await Post("Mine");
            await Post("Theirs", EmployerB);

            var result = await new GetMyJobsHandler(jobs).Handle(EmployerA, JobStatusFilter.All);

            Assert.Equal("Mine", Assert.Single(result).Title);
        }
    }
}
