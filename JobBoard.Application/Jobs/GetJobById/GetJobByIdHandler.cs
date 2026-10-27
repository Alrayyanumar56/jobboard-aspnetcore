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

        // Returns null when no job has this id; the controller turns that into a 404.
        public async Task<JobResponse?> Handle(int id)
        {
            var job = await jobRepository.GetJobById(id);
            return job?.ToResponse();
        }
    }
}
