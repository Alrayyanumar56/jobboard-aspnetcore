using JobBoard.Application.DTOs;
using JobBoard.Application.EmployerProfiles;
using JobBoard.Application.EmployerProfiles.CreateEmployerProfile;
using JobBoard.Application.EmployerProfiles.GetMyEmployerProfile;
using JobBoard.Application.EmployerProfiles.UpdateEmployerProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobBoard.Api.Controllers
{
    // An Employer's own company profile. Every route works on "the caller's profile": there is no
    // profile or user id in any URL or body, so there is nobody else's profile to ask for.
    // The public part (CompanyName) reaches job seekers through the job responses.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Employer")]
    public class EmployerProfileController : ControllerBase
    {
        private readonly GetMyEmployerProfileHandler getMyEmployerProfileHandler;
        private readonly CreateEmployerProfileHandler createEmployerProfileHandler;
        private readonly UpdateEmployerProfileHandler updateEmployerProfileHandler;

        public EmployerProfileController(GetMyEmployerProfileHandler getMyEmployerProfileHandler, CreateEmployerProfileHandler createEmployerProfileHandler, UpdateEmployerProfileHandler updateEmployerProfileHandler)
        {
            this.getMyEmployerProfileHandler = getMyEmployerProfileHandler;
            this.createEmployerProfileHandler = createEmployerProfileHandler;
            this.updateEmployerProfileHandler = updateEmployerProfileHandler;
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(EmployerProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile()
        {
            // Whose profile to return is decided ONLY by the validated token.
            var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(employerId))
            {
                return Unauthorized();
            }

            var profile = await getMyEmployerProfileHandler.Handle(employerId);
            if (profile == null)
            {
                return NotFound();
            }
            return Ok(profile);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmployerProfileResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateMyProfile(EmployerProfileCommand command)
        {
            // The owner is identified ONLY by the validated token. The body has no EmployerId.
            var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(employerId))
            {
                return Unauthorized();
            }

            var profile = await createEmployerProfileHandler.Handle(command, employerId);
            if (profile == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Profile already exists",
                    detail: "You already have a company profile. Update it instead.");
            }
            return StatusCode(StatusCodes.Status201Created, profile);
        }

        [HttpPut("me")]
        [ProducesResponseType(typeof(EmployerProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMyProfile(EmployerProfileCommand command)
        {
            // The owner is identified ONLY by the validated token. The body has no EmployerId.
            var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(employerId))
            {
                return Unauthorized();
            }

            var profile = await updateEmployerProfileHandler.Handle(command, employerId);
            if (profile == null)
            {
                return NotFound();
            }
            return Ok(profile);
        }
    }
}
