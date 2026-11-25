using JobBoard.Application.DTOs;
using JobBoard.Application.Jobs.ApplyToJob;
using JobBoard.Application.Jobs.CloseJob;
using JobBoard.Application.Jobs.CreateJob;
using JobBoard.Application.Jobs.GetJobById;
using JobBoard.Application.Jobs.GetJobApplicants;
using JobBoard.Application.Jobs.GetJobs;
using JobBoard.Application.Jobs.GetMyApplications;
using JobBoard.Application.Jobs.GetMyJobs;
using JobBoard.Application.Jobs.UpdateJob;
using JobBoard.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        private readonly UpdateJobHandler updateJobHandler;
        private readonly CloseJobHandler closeJobHandler;
        private readonly GetMyJobsHandler getMyJobsHandler;

        public JobController(CreateJobHandler createJobHandler, GetJobsHandler getJobsHandler, GetJobByIdHandler getJobByIdHandler, ApplyToJobHandler applyToJobHandler, GetMyApplicationsHandler getMyApplicationsHandler, GetJobApplicantsHandler getJobApplicantsHandler, UpdateJobHandler updateJobHandler, CloseJobHandler closeJobHandler, GetMyJobsHandler getMyJobsHandler)
        {
            this.createJobHandler = createJobHandler;
            this.getJobsHandler = getJobsHandler;
            this.getJobByIdHandler = getJobByIdHandler;
            this.applyToJobHandler = applyToJobHandler;
            this.getMyApplicationsHandler = getMyApplicationsHandler;
            this.getJobApplicantsHandler = getJobApplicantsHandler;
            this.updateJobHandler = updateJobHandler;
            this.closeJobHandler = closeJobHandler;
            this.getMyJobsHandler = getMyJobsHandler;
        }
        // Public browsing (docs/03-requirements.md): the class-level [Authorize] is overridden here.
        // GET /api/Job?jobType=FullTime&location=Remote&search=dotnet&page=1&pageSize=20
        // All optional. An unknown enum value (jobType=Banana), page < 1 or pageSize outside 1..50
        // fails validation and [ApiController] answers 400.
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedResult<JobResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetJobs([FromQuery] GetJobsQuery query)
        {
            var jobs = await getJobsHandler.Handle(query);
            return Ok(jobs);
        }

        // The calling employer's own jobs for their dashboard: ?status=open|closed|all (default all).
        // "mine" is a fixed segment, so it can't clash with GET {id:int}.
        [HttpGet("mine")]
        [Authorize(Roles = "Employer")]
        [ProducesResponseType(typeof(List<MyJobResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetMyJobs([FromQuery] JobStatusFilter status = JobStatusFilter.All)
        {
            // Whose jobs to return is decided ONLY by the validated token.
            var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(employerId))
            {
                return Unauthorized();
            }

            var jobs = await getMyJobsHandler.Handle(employerId, status);
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ApplyToJob(int jobId, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ApplyToJobCommand? command)
        {
            // The applicant is identified ONLY by the validated token, never by the request.
            var jobSeekerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(jobSeekerId))
            {
                return Unauthorized();
            }

            // The body is optional: no body (as before) means no cover letter.
            var result = await applyToJobHandler.Handle(jobId, jobSeekerId, command);
            return result.Status switch
            {
                ApplyToJobStatus.Created => StatusCode(StatusCodes.Status201Created, result.Application),
                ApplyToJobStatus.JobNotFound => NotFound(),
                ApplyToJobStatus.AlreadyApplied => Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Already applied",
                    detail: "You have already applied to this job."),
                ApplyToJobStatus.DeadlinePassed => Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Application deadline has passed",
                    detail: "This job no longer accepts applications."),
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

        [HttpPut("{jobId:int}")]
        [Authorize(Roles = "Employer")]
        [ProducesResponseType(typeof(JobResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateJob(int jobId, UpdateJobCommand command)
        {
            // The employer is identified ONLY by the validated token. The body has no EmployerId.
            // Ownership is checked in the handler; someone else's job looks exactly like a missing one.
            var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(employerId))
            {
                return Unauthorized();
            }

            var result = await updateJobHandler.Handle(jobId, command, employerId);
            return result.Status switch
            {
                UpdateJobStatus.Updated => Ok(result.Job),
                UpdateJobStatus.JobNotFound => NotFound(),
                UpdateJobStatus.JobClosed => Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Job is closed",
                    detail: "A closed job can't be edited."),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        // Closes (archives) the job instead of deleting it: the row and every application on it are kept.
        [HttpPost("{jobId:int}/close")]
        [Authorize(Roles = "Employer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CloseJob(int jobId)
        {
            var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(employerId))
            {
                return Unauthorized();
            }

            var closed = await closeJobHandler.Handle(jobId, employerId);
            if (!closed)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
