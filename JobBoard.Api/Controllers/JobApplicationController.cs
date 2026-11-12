using JobBoard.Application.DTOs;
using JobBoard.Application.Jobs;
using JobBoard.Application.Jobs.GetApplicationHistory;
using JobBoard.Application.Jobs.UpdateApplicationStatus;
using JobBoard.Application.Jobs.WithdrawApplication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobBoard.Api.Controllers
{
    // Operations on ONE application, addressed by its own id: api/JobApplication/{applicationId}/...
    // (Lists of applications stay under JobController, because they are scoped by job or by caller.)
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobApplicationController : ControllerBase
    {
        private readonly UpdateApplicationStatusHandler updateApplicationStatusHandler;
        private readonly WithdrawApplicationHandler withdrawApplicationHandler;
        private readonly GetApplicationHistoryHandler getApplicationHistoryHandler;

        public JobApplicationController(UpdateApplicationStatusHandler updateApplicationStatusHandler, WithdrawApplicationHandler withdrawApplicationHandler, GetApplicationHistoryHandler getApplicationHistoryHandler)
        {
            this.updateApplicationStatusHandler = updateApplicationStatusHandler;
            this.withdrawApplicationHandler = withdrawApplicationHandler;
            this.getApplicationHistoryHandler = getApplicationHistoryHandler;
        }

        [HttpPatch("{applicationId:int}/status")]
        [Authorize(Roles = "Employer")]
        [ProducesResponseType(typeof(ApplicationStatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateStatus(int applicationId, UpdateApplicationStatusCommand command)
        {
            // The employer is identified ONLY by the validated token. Ownership of the job this
            // application belongs to is checked in the handler.
            var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(employerId))
            {
                return Unauthorized();
            }

            var result = await updateApplicationStatusHandler.Handle(applicationId, command, employerId);
            return ToActionResult(result);
        }

        [HttpPatch("{applicationId:int}/withdraw")]
        [Authorize(Roles = "JobSeeker")]
        [ProducesResponseType(typeof(ApplicationStatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Withdraw(int applicationId)
        {
            // The applicant is identified ONLY by the validated token. There is no body: the only
            // thing a JobSeeker can do to an application is withdraw it.
            var jobSeekerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(jobSeekerId))
            {
                return Unauthorized();
            }

            var result = await withdrawApplicationHandler.Handle(applicationId, jobSeekerId);
            return ToActionResult(result);
        }

        [HttpGet("{applicationId:int}/history")]
        [Authorize(Roles = "Employer,JobSeeker")]
        [ProducesResponseType(typeof(List<ApplicationStatusHistoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHistory(int applicationId)
        {
            // The caller and their role come ONLY from the validated token. Whether this application
            // is theirs (their own, or one of their job's) is checked in the handler.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var history = await getApplicationHistoryHandler.Handle(applicationId, userId, User.IsInRole("Employer"));

            // Missing and not-yours both land here, so the response never reveals which ids exist.
            if (history == null)
            {
                return NotFound();
            }

            return Ok(history);
        }

        private IActionResult ToActionResult(ChangeApplicationStatusResult result)
        {
            return result.Outcome switch
            {
                ChangeApplicationStatusOutcome.Changed => Ok(result.Application),
                ChangeApplicationStatusOutcome.NotFound => NotFound(),
                ChangeApplicationStatusOutcome.InvalidTransition => Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Invalid status change",
                    detail: result.Message),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
