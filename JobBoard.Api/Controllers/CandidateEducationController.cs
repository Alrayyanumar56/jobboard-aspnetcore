using JobBoard.Application.DTOs;
using JobBoard.Application.Education;
using JobBoard.Application.Education.CreateCandidateEducation;
using JobBoard.Application.Education.DeleteCandidateEducation;
using JobBoard.Application.Education.GetMyCandidateEducation;
using JobBoard.Application.Education.UpdateCandidateEducation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobBoard.Api.Controllers
{
    // A JobSeeker's own education records. "Whose records" is never in a URL or body: it always comes
    // from the token. The {id} in PUT and DELETE only says WHICH of the caller's records, and a record
    // that isn't theirs behaves exactly like one that doesn't exist (404).
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "JobSeeker")]
    public class CandidateEducationController : ControllerBase
    {
        private readonly CreateCandidateEducationHandler createCandidateEducationHandler;
        private readonly GetMyCandidateEducationHandler getMyCandidateEducationHandler;
        private readonly UpdateCandidateEducationHandler updateCandidateEducationHandler;
        private readonly DeleteCandidateEducationHandler deleteCandidateEducationHandler;

        public CandidateEducationController(CreateCandidateEducationHandler createCandidateEducationHandler, GetMyCandidateEducationHandler getMyCandidateEducationHandler, UpdateCandidateEducationHandler updateCandidateEducationHandler, DeleteCandidateEducationHandler deleteCandidateEducationHandler)
        {
            this.createCandidateEducationHandler = createCandidateEducationHandler;
            this.getMyCandidateEducationHandler = getMyCandidateEducationHandler;
            this.updateCandidateEducationHandler = updateCandidateEducationHandler;
            this.deleteCandidateEducationHandler = deleteCandidateEducationHandler;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CandidateEducationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateMyEducation(CandidateEducationCommand command)
        {
            // The owner is identified ONLY by the validated token. The body has no user or profile id.
            var jobSeekerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(jobSeekerId))
            {
                return Unauthorized();
            }

            var education = await createCandidateEducationHandler.Handle(command, jobSeekerId);
            if (education == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Profile required",
                    detail: "Create your candidate profile before adding education.");
            }
            return StatusCode(StatusCodes.Status201Created, education);
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(List<CandidateEducationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetMyEducation()
        {
            // Whose records to return is decided ONLY by the validated token.
            var jobSeekerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(jobSeekerId))
            {
                return Unauthorized();
            }

            var records = await getMyCandidateEducationHandler.Handle(jobSeekerId);
            return Ok(records);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CandidateEducationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMyEducation(int id, CandidateEducationCommand command)
        {
            // The owner is identified ONLY by the validated token; {id} just picks one of their records.
            var jobSeekerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(jobSeekerId))
            {
                return Unauthorized();
            }

            var education = await updateCandidateEducationHandler.Handle(id, command, jobSeekerId);

            // Missing and not-yours both land here, so the response never reveals which ids exist.
            if (education == null)
            {
                return NotFound();
            }
            return Ok(education);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMyEducation(int id)
        {
            // The owner is identified ONLY by the validated token; {id} just picks one of their records.
            var jobSeekerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(jobSeekerId))
            {
                return Unauthorized();
            }

            var deleted = await deleteCandidateEducationHandler.Handle(id, jobSeekerId);

            // Missing and not-yours both land here, so the response never reveals which ids exist.
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
