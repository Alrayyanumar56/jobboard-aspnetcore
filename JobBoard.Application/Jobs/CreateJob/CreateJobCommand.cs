using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobBoard.Application.Jobs.CreateJob
{
    public class CreateJobHandler
    {
        private readonly IJobRepository jobRepository;

        public CreateJobHandler(IJobRepository jobRepository)
        {
            this.jobRepository = jobRepository;
        }

        // employerId comes from the authenticated user's JWT (resolved in the controller),
        // never from the request body.
        public async Task<JobResponse> Handle(CreateJobCommand command, string employerId)
        {
            var job = new Job
            {
                Title = command.Title.Trim(),
                Description = command.Description.Trim(),
                JobType = command.JobType!.Value,
                JobLocation = command.JobLocation!.Value,
                SalaryRange = string.IsNullOrWhiteSpace(command.SalaryRange) ? null : command.SalaryRange.Trim(),
                EmployerId = employerId,
                PostedAt = DateTimeOffset.UtcNow
            };

            var saved = await jobRepository.AddJob(job);

            return new JobResponse
            {
                Id = saved.Id,
                Title = saved.Title,
                Description = saved.Description,
                JobType = saved.JobType,
                JobLocation = saved.JobLocation,
                SalaryRange = saved.SalaryRange,
                EmployerId = saved.EmployerId,
                PostedAt = saved.PostedAt
            };
        }
    }
}
