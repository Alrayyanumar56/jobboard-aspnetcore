using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            var result = await registrationService.RegisterUser(registerRequest);
            if (result.Succeeded)
            {
                return StatusCode(201, "User Registered!");
            }
            else
            {
                return BadRequest("Registration Failed");
            }
        }
        [HttpPost("login")]
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
    }
}
