using JobBoard.Application.DTOs;
using JobBoard.Application.Jobs.CreateJob;
using JobBoard.Application.Jobs.GetJobById;
using JobBoard.Application.Jobs.GetJobs;
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

        public JobController(CreateJobHandler createJobHandler, GetJobsHandler getJobsHandler, GetJobByIdHandler getJobByIdHandler)
        {
            this.createJobHandler = createJobHandler;
            this.getJobsHandler = getJobsHandler;
            this.getJobByIdHandler = getJobByIdHandler;
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
    }
}
