using JobBoard.Application.DTOs;
using JobBoard.Application.Skills.SearchSkills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.Api.Controllers
{
    // The shared skill vocabulary. Skills are created implicitly when a job or profile names a new one;
    // this controller only reads them.
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class SkillController : ControllerBase
    {
        private readonly SearchSkillsHandler searchSkillsHandler;

        public SkillController(SearchSkillsHandler searchSkillsHandler)
        {
            this.searchSkillsHandler = searchSkillsHandler;
        }

        // GET /api/Skill?search=net -> up to 20 skills whose name contains "net", alphabetical.
        [HttpGet]
        [ProducesResponseType(typeof(List<SkillResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromQuery][StringLength(50)] string? search)
        {
            return Ok(await searchSkillsHandler.Handle(search));
        }
    }
}
