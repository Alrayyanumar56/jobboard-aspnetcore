using JobBoard.Application.DTOs;
using JobBoard.Application.Profiles;
using JobBoard.Application.Profiles.CreateCandidateProfile;
using JobBoard.Application.Profiles.GetMyCandidateProfile;
using JobBoard.Application.Profiles.UpdateCandidateProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobBoard.Api.Controllers
{
    // A JobSeeker's own core profile. Every route works on "the caller's profile": there is no profile
    // or user id in any URL or body, so there is nobody else's profile to ask for.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "JobSeeker")]
    public class CandidateProfileController : ControllerBase
    {
        private readonly GetMyCandidateProfileHandler getMyCandidateProfileHandler;
        private readonly CreateCandidateProfileHandler createCandidateProfileHandler;
        private readonly UpdateCandidateProfileHandler updateCandidateProfileHandler;

        public CandidateProfileController(GetMyCandidateProfileHandler getMyCandidateProfileHandler, CreateCandidateProfileHandler createCandidateProfileHandler, UpdateCandidateProfileHandler updateCandidateProfileHandler)
        {
            this.getMyCandidateProfileHandler = getMyCandidateProfileHandler;
            this.createCandidateProfileHandler = createCandidateProfileHandler;
            this.updateCandidateProfileHandler = updateCandidateProfileHandler;
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(CandidateProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile()
        {
            // Whose profile to return is decided ONLY by the validated token.
            var jobSeekerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(jobSeekerId))
            {
                return Unauthorized();
            }

            var profile = await getMyCandidateProfileHandler.Handle(jobSeekerId);
            if (profile == null)
            {
                return NotFound();
            }
            return Ok(profile);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CandidateProfileResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateMyProfile(CandidateProfileCommand command)
        {
            // The owner is identified ONLY by the validated token. The body has no JobSeekerId.
            var jobSeekerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(jobSeekerId))
            {
                return Unauthorized();
            }

            var profile = await createCandidateProfileHandler.Handle(command, jobSeekerId);
            if (profile == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Profile already exists",
                    detail: "You already have a profile. Update it instead.");
            }
            return StatusCode(StatusCodes.Status201Created, profile);
        }

        [HttpPut("me")]
        [ProducesResponseType(typeof(CandidateProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMyProfile(CandidateProfileCommand command)
        {
            // The owner is identified ONLY by the validated token. The body has no JobSeekerId.
            var jobSeekerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(jobSeekerId))
            {
                return Unauthorized();
            }

            var profile = await updateCandidateProfileHandler.Handle(command, jobSeekerId);
            if (profile == null)
            {
                return NotFound();
            }
            return Ok(profile);
        }
    }
}
