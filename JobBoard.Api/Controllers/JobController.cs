using JobBoard.Application.DTOs;
using JobBoard.Application.Jobs.CreateJob;
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

        public JobController(CreateJobHandler createJobHandler)
        {
            this.createJobHandler = createJobHandler;
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
