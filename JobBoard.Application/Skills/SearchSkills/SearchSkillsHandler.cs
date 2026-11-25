using JobBoard.Application.DTOs;
using JobBoard.Application.Interfaces;

namespace JobBoard.Application.Skills.SearchSkills
{
    public class SearchSkillsHandler
    {
        public const int MaxResults = 20;

        private readonly ISkillRepository skillRepository;

        public SearchSkillsHandler(ISkillRepository skillRepository)
        {
            this.skillRepository = skillRepository;
        }

        // Autocomplete for skill inputs, so clients reuse existing skills instead of near-duplicates.
        public Task<List<SkillResponse>> Handle(string? search) =>
            skillRepository.Search(search, MaxResults);
    }
}
