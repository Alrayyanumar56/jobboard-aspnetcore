using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;

namespace JobBoard.Application.Jobs.GetJobById
{
    public class GetJobByIdHandler
    {
        private readonly IJobRepository jobRepository;

        public GetJobByIdHandler(IJobRepository jobRepository)
        {
            this.jobRepository = jobRepository;
        }

        // Returns null when no job has this id, or the job is closed; the controller turns that into a 404.
        // A job past its ApplicationDeadline is still returned (it is only hidden from the listing), so a
        // seeker following a link from "my applications" still sees it.
        public async Task<JobResponse?> Handle(int id)
        {
            return await jobRepository.GetJobDetails(id, openOnly: true);
        }
    }
}
