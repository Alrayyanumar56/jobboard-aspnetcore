using JobBoard.Application.DTOs;
using JobBoard.Application.Jobs.ApplyToJob;
using JobBoard.Application.Jobs.CreateJob;
using JobBoard.Application.Jobs.GetJobById;
using JobBoard.Application.Jobs.GetJobApplicants;
using JobBoard.Application.Jobs.GetJobs;
using JobBoard.Application.Jobs.GetMyApplications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobBoard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobController : ControllerBase
    {
        private readonly CreateJobHandler createJobHandler;
        private readonly GetJobsHandler getJobsHandler;
        private readonly GetJobByIdHandler getJobByIdHandler;
        private readonly ApplyToJobHandler applyToJobHandler;
        private readonly GetMyApplicationsHandler getMyApplicationsHandler;
        private readonly GetJobApplicantsHandler getJobApplicantsHandler;

        public JobController(CreateJobHandler createJobHandler, GetJobsHandler getJobsHandler, GetJobByIdHandler getJobByIdHandler, ApplyToJobHandler applyToJobHandler, GetMyApplicationsHandler getMyApplicationsHandler, GetJobApplicantsHandler getJobApplicantsHandler)
        {
            this.createJobHandler = createJobHandler;
            this.getJobsHandler = getJobsHandler;
            this.getJobByIdHandler = getJobByIdHandler;
            this.applyToJobHandler = applyToJobHandler;
            this.getMyApplicationsHandler = getMyApplicationsHandler;
            this.getJobApplicantsHandler = getJobApplicantsHandler;
        }
        // Public browsing (docs/03-requirements.md): the class-level [Authorize] is overridden here.
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<JobResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetJobs()
        {
            var jobs = await getJobsHandler.Handle();
            return Ok(jobs);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(JobResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetJobById(int id)
        {
            var job = await getJobByIdHandler.Handle(id);
            if (job == null)
            {
                return NotFound();
            }
            return Ok(job);
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        [ProducesResponseType(typeof(JobResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateJob(CreateJobCommand command)
        {
            // Identity of the employer comes ONLY from the validated token.
            var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(employerId))
            {
                return Unauthorized();
            }

            var result = await createJobHandler.Handle(command, employerId);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        [HttpPost("{jobId:int}/applications")]
        [Authorize(Roles = "JobSeeker")]
        [ProducesResponseType(typeof(JobApplicationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ApplyToJob(int jobId)
        {
            // The applicant is identified ONLY by the validated token, never by the request.
            var jobSeekerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(jobSeekerId))
            {
                return Unauthorized();
            }

            var result = await applyToJobHandler.Handle(jobId, jobSeekerId);
            return result.Status switch
            {
                ApplyToJobStatus.Created => StatusCode(StatusCodes.Status201Created, result.Application),
                ApplyToJobStatus.JobNotFound => NotFound(),
                ApplyToJobStatus.AlreadyApplied => Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Already applied",
                    detail: "You have already applied to this job."),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
        // "my-applications" is a fixed segment, so it can't clash with GET {id:int}.
        [HttpGet("my-applications")]
        [Authorize(Roles = "JobSeeker")]
        [ProducesResponseType(typeof(List<MyApplicationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetMyApplications()
        {
            // Whose applications to return is decided ONLY by the validated token.
            var jobSeekerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(jobSeekerId))
            {
                return Unauthorized();
            }

            var applications = await getMyApplicationsHandler.Handle(jobSeekerId);
            return Ok(applications);
        }

        // Same path as POST {jobId}/applications; the HTTP verb tells them apart.
        [HttpGet("{jobId:int}/applications")]
        [Authorize(Roles = "Employer")]
        [ProducesResponseType(typeof(List<JobApplicantResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetJobApplicants(int jobId)
        {
            // The employer is identified ONLY by the validated token. Ownership of the job
            // is checked in the handler; a job that isn't theirs looks exactly like a missing job.
            var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(employerId))
            {
                return Unauthorized();
            }

            var applicants = await getJobApplicantsHandler.Handle(jobId, employerId);
            if (applicants == null)
            {
                return NotFound();
            }
            return Ok(applicants);
        }
    }
}
