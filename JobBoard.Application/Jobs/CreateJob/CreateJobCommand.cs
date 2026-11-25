using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Entities;
using JobBoard.Domain.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobBoard.Application.Jobs.CreateJob
{
    public class CreateJobHandler
    {
        private readonly IJobRepository jobRepository;
        private readonly ISkillRepository skillRepository;

        public CreateJobHandler(IJobRepository jobRepository, ISkillRepository skillRepository)
        {
            this.jobRepository = jobRepository;
            this.skillRepository = skillRepository;
        }

        // employerId comes from the authenticated user's JWT (resolved in the controller),
        // never from the request body.
        public async Task<JobResponse> Handle(CreateJobCommand command, string employerId)
        {
            // Skill names -> shared Skill rows (new ones are created), so jobs and candidates use the same ids.
            var skills = await skillRepository.GetOrCreate(SkillNameRules.CleanList(command.Skills));

            var job = new Job
            {
                Title = command.Title.Trim(),
                Description = command.Description.Trim(),
                JobType = command.JobType!.Value,
                JobLocation = command.JobLocation!.Value,
                SalaryRange = string.IsNullOrWhiteSpace(command.SalaryRange) ? null : command.SalaryRange.Trim(),
                City = string.IsNullOrWhiteSpace(command.City) ? null : command.City.Trim(),
                ExperienceLevel = command.ExperienceLevel!.Value,
                ApplicationDeadline = command.ApplicationDeadline,
                EmployerId = employerId,
                PostedAt = DateTimeOffset.UtcNow
            };

            var saved = await jobRepository.AddJob(job, skills.Select(s => s.Id).ToList());

            // Read back in the public shape, so the response carries the company name and skills
            // exactly as GET /api/Job/{id} will show them.
            return (await jobRepository.GetJobDetails(saved.Id, openOnly: false))!;
        }
    }
}
