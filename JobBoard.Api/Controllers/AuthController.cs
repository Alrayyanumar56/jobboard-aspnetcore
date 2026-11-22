using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobBoard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRegistrationService registrationService;
        private readonly ILoginService loginService;

        public AuthController(IRegistrationService registrationService,ILoginService loginService)
        {
            this.registrationService = registrationService;
            this.loginService = loginService;
        }
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            var result = await registrationService.RegisterUser(registerRequest);
            if (result.Succeeded)
            {
                return StatusCode(StatusCodes.Status201Created, "User Registered!");
            }

            // Same 400 shape as a model-validation failure, with Identity's actual reasons
            // ("Username is already taken", "Passwords must have at least one digit"...).
            foreach (var (field, messages) in result.Errors)
            {
                foreach (var message in messages)
                {
                    ModelState.AddModelError(field, message);
                }
            }
            return ValidationProblem(ModelState);
        }
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var result = await loginService.LoginUser(loginRequest);
            if (result!=null)
            {
                return Ok(result);
            }
            else
            {
                return Unauthorized();
            }

        }

        // Who the caller is, read from the validated token only. A frontend calls this after a page
        // refresh to restore its session (and to find out the token has expired: 401).
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || !Enum.TryParse<UserRole>(role, out var parsedRole))
            {
                return Unauthorized();
            }

            return Ok(new CurrentUserResponse
            {
                UserId = userId,
                Email = email,
                Role = parsedRole
            });
        }
    }
}
