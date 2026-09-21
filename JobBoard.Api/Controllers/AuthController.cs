using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;
using JobBoard.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRegistrationService registrationService;

        public AuthController(IRegistrationService registrationService)
        {
            this.registrationService = registrationService;
        }
        [HttpPost(Name = "RegisterRequest")]
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
    }
}
